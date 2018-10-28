using Nitro.Composer.Instruments;
using Nitro.Composer.SequenceCommands;
using Nitro.Composer.SequenceCommands.Rand;
using Nitro.Composer.SequenceCommands.Var;
using System;
using System.Collections.Generic;

namespace NitroComposerPlayer {
	internal class TrackPlayer {
		internal SequencePlayer sequencePlayer;

		private uint nextInstructionId;
		public bool endFlag;

		private bool noteWait = true;

		private bool tieMode;
		private ChannelInfo tieChannel;

		internal bool portamentoEnabled;
		internal byte portamentoKey;
		internal int portamentoTime;

		private uint waitTimer;

		private bool conditionFlag = false;

		internal Instrument instrument;

		private int Prio;

		internal byte Volume = 0x7F;
		internal byte Expression = 0x7F;

		internal byte PitchBendRange;
		internal byte PitchBend;
		internal byte Transpose;

		byte AttackOverride = 0xFF;
		byte DecayOverride = 0xFF;
		byte SustainOverride = 0xFF;
		byte ReleaseOverride = 0xFF;

		internal int ModulationDelay;
		internal int ModulationDepth;
		internal int ModulationRange;
		internal int ModulationSpeed;
		internal ModulationTypeEnum ModulationType;

		internal ushort SweepPitch;

		private Stack<uint> callStack = new Stack<uint>();
		private Stack<LoopEntry> loopStack = new Stack<LoopEntry>();
		
		internal TrackUpdateFlags updateFlags;

		internal int Pan;

		public TrackPlayer(SequencePlayer sequencePlayer, uint nextInstructionId = 0) {
			this.sequencePlayer = sequencePlayer;
			this.nextInstructionId = nextInstructionId;

			Prio = 0;
			instrument = sequencePlayer.bank.instruments[0];
		}

		private void NoteOn(byte note, uint velocity, uint duration) {
			if(noteWait) {
				waitTimer = duration;
			}
			if(tieMode) {
				NoteOnTie(note, velocity);
			} else {
				NoteOnNormal(note, velocity, duration);
			}
		}

		private ChannelInfo NoteOnNormal(byte note, uint velocity, uint duration) {
			var leafInstrument = instrument.leafInstrumentForNote(note);
			ChannelInfo channel = sequencePlayer.FindChannelForInstrument(leafInstrument);

			if(channel == null) return null;

			channel.Track = this;
			channel.Instrument = leafInstrument;

			channel.Prio = this.Prio;

			channel.Note = (byte)(note + Transpose);
			channel.Duration = duration;

			channel.AttackLevel = Remap.Attack(AttackOverride != 0xFF ? AttackOverride : leafInstrument.Attack);
			channel.DecayRate = Remap.Rate(DecayOverride != 0xFF ? DecayOverride : leafInstrument.Decay);
			channel.SustainLevel = Remap.Level(SustainOverride != 0xFF ? SustainOverride : leafInstrument.Sustain);
			channel.ReleaseRate = Remap.Rate(ReleaseOverride != 0xFF ? ReleaseOverride : leafInstrument.Release);

			channel.state = ChannelInfo.ChannelState.Start;

			channel.ModulationStartCounter = 0;
			channel.ModulationCounter = 0;

			return channel;
		}

		private void NoteOnTie(byte note, uint velocity) {
			if(tieChannel == null) {
				tieChannel = NoteOnNormal(note, velocity, 0);
				return;
			}

			tieChannel.Prio = this.Prio;
			tieChannel.Note = (byte)(note + Transpose);
			tieChannel.Velocity = velocity;
			tieChannel.ModulationStartCounter = 0;
			tieChannel.ModulationCounter = 0;
		}

		private short Rand(short min, short max) {
			throw new NotImplementedException();
		}
		private short Var(uint varId) {
			return sequencePlayer.Variables[varId];
		}
		private void SetVar(byte variable, short v) {
			sequencePlayer.Variables[variable] = v;
		}

		private void DoPan(byte pan) {
			this.Pan = pan - 64;
			updateFlags |= TrackUpdateFlags.Pan;
		}

		private void ReleaseAllNotes() {
			tieChannel = null;
			foreach(var chan in sequencePlayer.channels) {
				if(chan.Track != this) continue;
				if(chan.state == ChannelInfo.ChannelState.None) continue;
				if(chan.state == ChannelInfo.ChannelState.Release) continue;
				chan.Release();
			}
		}

		/** Returns true if another command should be executed right away and false if there is a delay to wait out */
		internal bool ExecuteNextCommand() {
			dynamic command = sequencePlayer.sseq.sequence.commands[(int)nextInstructionId++];
			if(command.Conditional && !conditionFlag) return true;
			ExecuteCommand(command);

			return waitTimer == 0;
		}

		#region Command handlers

		private void ExecuteCommand(BaseSequenceCommand cmd) {
			throw new NotImplementedException();
		}

		private void ExecuteCommand(RestCommand cmd) {
			waitTimer = cmd.Rest;
		}

		private void ExecuteCommand(ProgramChangeCommand cmd) {
			instrument = sequencePlayer.bank.instruments[(int)cmd.Program];
		}

		private void ExecuteCommand(NoteCommand cmd) {
			NoteOn(cmd.Note, cmd.Velocity, cmd.Duration);
		}
		private void ExecuteCommand(NoteRandCommand cmd) {
			uint duration = (uint)Rand(cmd.DurationMin, cmd.DurationMax);
			NoteOn(cmd.Note, cmd.Velocity, duration);
		}
		private void ExecuteCommand(NoteVarCommand cmd) {
			uint duration = (uint)Var(cmd.DurationVar);
			NoteOn(cmd.Note, cmd.Velocity, duration);
		}

		private void ExecuteCommand(AllocateTracksCommand cmd) {
		}

		private void ExecuteCommand(OpenTrackCommand cmd) {
			sequencePlayer.tracks[cmd.Track] = new TrackPlayer(sequencePlayer, cmd.target);
		}

		private void ExecuteCommand(TempoCommand cmd) {
			sequencePlayer.Tempo = cmd.Tempo;
		}

		private void ExecuteCommand(EndTrackCommand cmd) {
			endFlag = true;
		}

		private void ExecuteCommand(JumpCommand cmd) {
			if(cmd.type == JumpCommand.JumpType.CALL) {
				callStack.Push(nextInstructionId);
			}
			nextInstructionId = cmd.target;
		}

		private void ExecuteCommand(VolumeCommand cmd) {
			if(cmd.Master) {
				sequencePlayer.MasterVolume = cmd.Volume;
			} else {
				Volume = cmd.Volume;
			}
			updateFlags |= TrackUpdateFlags.Volume;
		}
		private void ExecuteCommand(VolumeRandCommand cmd) {
			if(cmd.Master) {
				sequencePlayer.MasterVolume = (byte)Rand(cmd.VolumeMin, cmd.VolumeMax);
			} else {
				Volume = (byte)Rand(cmd.VolumeMin, cmd.VolumeMax);
			}
			updateFlags |= TrackUpdateFlags.Volume;
		}
		private void ExecuteCommand(VolumeVarCommand cmd) {
			if(cmd.Master) {
				sequencePlayer.MasterVolume = (byte)Var(cmd.VolumeVar);
			} else {
				Volume = (byte)Var(cmd.VolumeVar);
			}
			updateFlags |= TrackUpdateFlags.Volume;
		}

		private void ExecuteCommand(ExpressionCommand cmd) {
			Expression = cmd.Value;
			updateFlags |= TrackUpdateFlags.Volume;
		}
		private void ExecuteCommand(ExpressionRandCommand cmd) {
			Expression = (byte)Rand(cmd.Min, cmd.Max);
			updateFlags |= TrackUpdateFlags.Volume;
		}
		private void ExecuteCommand(ExpressionVarCommand cmd) {
			Expression = (byte)Var(cmd.Var);
			updateFlags |= TrackUpdateFlags.Volume;
		}

		private void ExecuteCommand(PanCommand cmd) {
			DoPan(cmd.Pan);
		}
		private void ExecuteCommand(PanVarCommand cmd) {
			DoPan((byte)Var(cmd.PanVar));
		}
		private void ExecuteCommand(PanRandCommand cmd) {
			DoPan((byte)Rand(cmd.PanMin, cmd.PanMax));
		}

		private void ExecuteCommand(PitchBendCommand cmd) {
			if(cmd.IsRange) {
				PitchBendRange = cmd.Bend;
			} else {
				PitchBend = cmd.Bend;
			}
			updateFlags |= TrackUpdateFlags.Timer;
		}
		private void ExecuteCommand(PitchBendRandCommand cmd) {
			if(cmd.IsRange) {
				PitchBendRange = (byte)Rand(cmd.BendMin,cmd.BendMax);
			} else {
				PitchBend = (byte)Rand(cmd.BendMin, cmd.BendMax);
			}
			updateFlags |= TrackUpdateFlags.Timer;
		}
		private void ExecuteCommand(PitchBendVarCommand cmd) {
			if(cmd.IsRange) {
				PitchBendRange = (byte)Var(cmd.BendVar);
			} else {
				PitchBend = (byte)Var(cmd.BendVar);
			}
			updateFlags |= TrackUpdateFlags.Timer;
		}

		private void ExecuteCommand(TransposeCommand cmd) {
			Transpose = cmd.Transpose;
		}
		private void ExecuteCommand(TransposeRandCommand cmd) {
			Transpose = (byte)Rand(cmd.TransposeMin, cmd.TransposeMax);
		}
		private void ExecuteCommand(TransposeVarCommand cmd) {
			Transpose = (byte)Var(cmd.TransposeVar);
		}

		private void ExecuteCommand(PortamentoCommand cmd) {
			portamentoEnabled = cmd.Enable;
		}

		private void ExecuteCommand(PortamentoKeyCommand cmd) {
			portamentoKey = (byte)(cmd.Key + Transpose);
		}

		private void ExecuteCommand(PortamentoTimeCommand cmd) {
			portamentoTime = cmd.Time;
		}
		private void ExecuteCommand(PortamentoTimeRandCommand cmd) {
			portamentoTime = Rand(cmd.TimeMin, cmd.TimeMax);
		}
		private void ExecuteCommand(PortamentoTimeVarCommand cmd) {
			portamentoTime = Var(cmd.TimeVar);
		}

		private void ExecuteCommand(ModulationCommand cmd) {
			int val = cmd.Value;
			switch(cmd.Type) {
				case ModulationCommand.ModType.DELAY:
					ModulationDelay = val;
					break;
				case ModulationCommand.ModType.DEPTH:
					ModulationDepth = val;
					break;
				case ModulationCommand.ModType.RANGE:
					ModulationRange = val;
					break;
				case ModulationCommand.ModType.SPEED:
					ModulationSpeed = val;
					break;
				case ModulationCommand.ModType.TYPE:
					ModulationType = (ModulationTypeEnum)val;
					break;
			}
			updateFlags |= TrackUpdateFlags.Modulation;
		}
		private void ExecuteCommand(ModulationRandCommand cmd) {
			int val = Rand(cmd.Min, cmd.Max);
			switch(cmd.Type) {
				case ModulationCommand.ModType.DELAY:
					ModulationDelay = val;
					break;
				case ModulationCommand.ModType.DEPTH:
					ModulationDepth = val;
					break;
				case ModulationCommand.ModType.RANGE:
					ModulationRange = val;
					break;
				case ModulationCommand.ModType.SPEED:
					ModulationSpeed = val;
					break;
				case ModulationCommand.ModType.TYPE:
					ModulationType = (ModulationTypeEnum)val;
					break;
			}
			updateFlags |= TrackUpdateFlags.Modulation;
		}
		private void ExecuteCommand(ModulationVarCommand cmd) {
			int val = Var(cmd.Var);
			switch(cmd.Type) {
				case ModulationCommand.ModType.DELAY:
					ModulationDelay = val;
					break;
				case ModulationCommand.ModType.DEPTH:
					ModulationDepth = val;
					break;
				case ModulationCommand.ModType.RANGE:
					ModulationRange = val;
					break;
				case ModulationCommand.ModType.SPEED:
					ModulationSpeed = val;
					break;
				case ModulationCommand.ModType.TYPE:
					ModulationType = (ModulationTypeEnum)val;
					break;
			}
			updateFlags |= TrackUpdateFlags.Modulation;
		}

		private void ExecuteCommand(ModulationDelayCommand cmd) {
			ModulationDelay = cmd.Delay;
			updateFlags |= TrackUpdateFlags.Modulation;
		}

		private void ExecuteCommand(SweepPitchCommand cmd) {
			SweepPitch = cmd.Ammount;
		}
		private void ExecuteCommand(SweepPitchRandCommand cmd) {
			SweepPitch = (ushort)Rand(cmd.AmmountMin,cmd.AmmountMax);
		}
		private void ExecuteCommand(SweepPitchVarCommand cmd) {
			SweepPitch = (ushort)Var(cmd.Var);
		}

		private void ExecuteCommand(ADSRCommand cmd) {
			switch(cmd.envPos) {
				case ADSRCommand.EnvPos.ATTACK:
					AttackOverride = cmd.Value;
					break;
				case ADSRCommand.EnvPos.DECAY:
					DecayOverride = cmd.Value;
					break;
				case ADSRCommand.EnvPos.SUSTAIN:
					SustainOverride = cmd.Value;
					break;
				case ADSRCommand.EnvPos.RELEASE:
					ReleaseOverride = cmd.Value;
					break;
			}
		}
		private void ExecuteCommand(ADSRRandCommand cmd) {
			switch(cmd.envPos) {
				case ADSRCommand.EnvPos.ATTACK:
					AttackOverride = (byte)Rand(cmd.Min,cmd.Max);
					break;
				case ADSRCommand.EnvPos.DECAY:
					DecayOverride = (byte)Rand(cmd.Min, cmd.Max);
					break;
				case ADSRCommand.EnvPos.SUSTAIN:
					SustainOverride = (byte)Rand(cmd.Min, cmd.Max);
					break;
				case ADSRCommand.EnvPos.RELEASE:
					ReleaseOverride = (byte)Rand(cmd.Min, cmd.Max);
					break;
			}
		}
		private void ExecuteCommand(ADSRVarCommand cmd) {
			switch(cmd.envPos) {
				case ADSRCommand.EnvPos.ATTACK:
					AttackOverride = (byte)Var(cmd.Var);
					break;
				case ADSRCommand.EnvPos.DECAY:
					DecayOverride = (byte)Var(cmd.Var);
					break;
				case ADSRCommand.EnvPos.SUSTAIN:
					SustainOverride = (byte)Var(cmd.Var);
					break;
				case ADSRCommand.EnvPos.RELEASE:
					ReleaseOverride = (byte)Var(cmd.Var);
					break;
			}
		}

		private void ExecuteCommand(MonoPolyCommand cmd) {
			noteWait = cmd.IsMono;
		}

		private void ExecuteCommand(TieCommand cmd) {
			tieMode = cmd.Tie;
			ReleaseAllNotes();
		}

		private void ExecuteCommand(PriorityCommand cmd) {
			this.Prio = cmd.Priority;
		}

		private void ExecuteCommand(LoopStartCommand cmd) {
			loopStack.Push(new LoopEntry(cmd.LoopCount, nextInstructionId));
		}

		private void ExecuteCommand(LoopEndCommand cmd) {
			var entry = loopStack.Peek();
			if(entry.loopCounter == 0) {
				loopStack.Pop();
			} else {
				entry.loopCounter--;
			}
		}

		private void ExecuteCommand(ReturnCommand cmd) {
			nextInstructionId = callStack.Pop();
		}

		private void ExecuteCommand(VarCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ADD:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) + cmd.Operand));
					break;
				case VarCommand.Operator.SUB:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) - cmd.Operand));
					break;
				case VarCommand.Operator.MUL:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) * cmd.Operand));
					break;
				case VarCommand.Operator.DIV:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) / cmd.Operand));
					break;
				case VarCommand.Operator.ASSIGN:
					SetVar(cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.SHIFT:
					if(cmd.Operand < 0) {
						SetVar(cmd.Variable, (short)(Var(cmd.Variable) >> (int)(-cmd.Operand)));
					} else {
						SetVar(cmd.Variable, (short)(Var(cmd.Variable) << (int)cmd.Operand));
					}
					break;
				case VarCommand.Operator.RAND:
					SetVar(cmd.Variable, Rand(0, cmd.Operand));
					break;

				case VarCommand.Operator.EQU:
					conditionFlag = Var(cmd.Variable) == cmd.Operand;
					break;
				case VarCommand.Operator.NEQ:
					conditionFlag = Var(cmd.Variable) != cmd.Operand;
					break;
				case VarCommand.Operator.GT:
					conditionFlag = Var(cmd.Variable) > cmd.Operand;
					break;
				case VarCommand.Operator.GTE:
					conditionFlag = Var(cmd.Variable) >= cmd.Operand;
					break;
				case VarCommand.Operator.LT:
					conditionFlag = Var(cmd.Variable) < cmd.Operand;
					break;
				case VarCommand.Operator.LTE:
					conditionFlag = Var(cmd.Variable) <= cmd.Operand;
					break;
			}
		}
		private void ExecuteCommand(VarVarCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ADD:
					SetVar(cmd.Variable1, (short)(Var(cmd.Variable1) + Var(cmd.Variable2)));
					break;
				case VarCommand.Operator.SUB:
					SetVar(cmd.Variable1, (short)(Var(cmd.Variable1) - Var(cmd.Variable2)));
					break;
				case VarCommand.Operator.MUL:
					SetVar(cmd.Variable1, (short)(Var(cmd.Variable1) * Var(cmd.Variable2)));
					break;
				case VarCommand.Operator.DIV:
					SetVar(cmd.Variable1, (short)(Var(cmd.Variable1) / Var(cmd.Variable2)));
					break;
				case VarCommand.Operator.ASSIGN:
					SetVar(cmd.Variable1, Var(cmd.Variable2));
					break;
				case VarCommand.Operator.SHIFT: {
					short shiftAmount = Var(cmd.Variable2);
					if(shiftAmount < 0) {
						SetVar(cmd.Variable1, (short)(Var(cmd.Variable1) >> (-shiftAmount)));
					} else {
						SetVar(cmd.Variable1, (short)(Var(cmd.Variable1) << shiftAmount));
					}
				}
				break;
				case VarCommand.Operator.RAND:
					SetVar(cmd.Variable1, Rand(0, Var(cmd.Variable2)));
					break;

				case VarCommand.Operator.EQU:
					conditionFlag = Var(cmd.Variable1) == Var(cmd.Variable2);
					break;
				case VarCommand.Operator.NEQ:
					conditionFlag = Var(cmd.Variable1) != Var(cmd.Variable2);
					break;
				case VarCommand.Operator.GT:
					conditionFlag = Var(cmd.Variable1) > Var(cmd.Variable2);
					break;
				case VarCommand.Operator.GTE:
					conditionFlag = Var(cmd.Variable1) >= Var(cmd.Variable2);
					break;
				case VarCommand.Operator.LT:
					conditionFlag = Var(cmd.Variable1) < Var(cmd.Variable2);
					break;
				case VarCommand.Operator.LTE:
					conditionFlag = Var(cmd.Variable1) <= Var(cmd.Variable2);
					break;
			}
		}
		private void ExecuteCommand(VarRandCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ADD:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) + Rand(cmd.OperandMin, cmd.OperandMax)));
					break;
				case VarCommand.Operator.SUB:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) - Rand(cmd.OperandMin, cmd.OperandMax)));
					break;
				case VarCommand.Operator.MUL:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) * Rand(cmd.OperandMin, cmd.OperandMax)));
					break;
				case VarCommand.Operator.DIV:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) / Rand(cmd.OperandMin, cmd.OperandMax)));
					break;
				case VarCommand.Operator.ASSIGN:
					SetVar(cmd.Variable, Rand(cmd.OperandMin, cmd.OperandMax));
					break;
				case VarCommand.Operator.SHIFT: {
					int shiftAmount = Rand(cmd.OperandMin, cmd.OperandMax);
					if(shiftAmount < 0) {
						SetVar(cmd.Variable, (short)(Var(cmd.Variable) >> -shiftAmount));
					} else {
						SetVar(cmd.Variable, (short)(Var(cmd.Variable) << shiftAmount));
					}
				}
				break;
				case VarCommand.Operator.RAND:
					SetVar(cmd.Variable, Rand(0, Rand(cmd.OperandMin, cmd.OperandMax)));
					break;

				case VarCommand.Operator.EQU:
					conditionFlag = Var(cmd.Variable) == Rand(cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.NEQ:
					conditionFlag = Var(cmd.Variable) != Rand(cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.GT:
					conditionFlag = Var(cmd.Variable) > Rand(cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.GTE:
					conditionFlag = Var(cmd.Variable) >= Rand(cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.LT:
					conditionFlag = Var(cmd.Variable) < Rand(cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.LTE:
					conditionFlag = Var(cmd.Variable) <= Rand(cmd.OperandMin, cmd.OperandMax);
					break;
			}
		}

		#endregion

		private class LoopEntry {
			public int loopCounter;
			public uint loopStart;

			public LoopEntry(int loopCount, uint loopStart) {
				loopCounter = loopCount;
				this.loopStart = loopStart;
			}
		}

		[Flags]
		internal enum TrackUpdateFlags {
			Volume = 1,
			Pan = 2,
			Timer = 4,
			Modulation = 8,
			Length = 16
		}

		internal enum ModulationTypeEnum {
			Pitch=0,
			Volume=1,
			Pan=2
		}
	}
}