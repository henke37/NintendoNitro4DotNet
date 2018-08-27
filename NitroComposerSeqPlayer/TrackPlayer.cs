using Nitro.Composer.Instruments;
using Nitro.Composer.SequenceCommands;
using Nitro.Composer.SequenceCommands.Rand;
using Nitro.Composer.SequenceCommands.Var;
using System;
using System.Collections.Generic;

namespace NitroComposerSeqPlayer {
	internal class TrackPlayer {
		private SequencePlayer sequencePlayer;

		private uint nextInstructionId;
		public bool endFlag;

		private bool noteWait;

		private bool tieMode;
		private ChannelInfo tieChannel;

		private bool portamentoEnabled;
		private byte portamentoKey;
		private int portamentoTime;

		private uint waitTimer;

		private bool conditionFlag = false;

		private Instrument instrument;

		private int Prio;

		private byte Volume = 0x7F;
		private byte Expression = 0x7F;

		byte AttackOverride = 0xFF;
		byte DecayOverride = 0xFF;
		byte SustainOverride = 0xFF;
		byte ReleaseOverride = 0xFF;

		private Stack<uint> callStack = new Stack<uint>();
		private Stack<LoopEntry> loopStack = new Stack<LoopEntry>();
		private byte PitchBendRange;
		private byte PitchBend;

		public TrackPlayer(SequencePlayer sequencePlayer, uint nextInstructionId = 0) {
			this.sequencePlayer = sequencePlayer;
			this.nextInstructionId = nextInstructionId;

			Prio = 0;
			instrument = sequencePlayer.bank.instruments[0];
		}

		private ChannelInfo NoteOn(byte note, uint velocity, uint duration) {
			var leafInstrument = instrument.leafInstrumentForNote(note);
			ChannelInfo channel = sequencePlayer.FindChannelForInstrument(leafInstrument);

			if(channel == null) return null;

			channel.Prio = this.Prio;

			channel.Duration = duration;

			channel.Attack = (AttackOverride != 0xFF ? AttackOverride : leafInstrument.Attack);
			channel.Decay = (DecayOverride != 0xFF ? DecayOverride : leafInstrument.Decay);
			channel.Sustain = (SustainOverride != 0xFF ? SustainOverride : leafInstrument.Sustain);
			channel.Release = (ReleaseOverride != 0xFF ? ReleaseOverride : leafInstrument.Release);

			channel.state = ChannelInfo.ChannelState.Start;

			channel.ModulationStartCounter = 0;
			channel.ModulationCounter = 0;

			return channel;
		}

		private void NoteOnTie(byte note, uint velocity) {
			if(tieChannel == null) {
				tieChannel = NoteOn(note, velocity, 0);
				return;
			}

			tieChannel.Prio = this.Prio;
			tieChannel.Note = note;
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

		private void Pan(byte pan) {
			throw new NotImplementedException();
		}

		private void ReleaseAllNotes() {
			throw new NotImplementedException();
		}

		/** Returns true if another command should be executed right away and false if there is a delay to wait out */
		internal bool ExecuteNextCommand() {
			dynamic command = sequencePlayer.sseq.sequence.commands[(int)nextInstructionId++];
			if(command.Conditional && !conditionFlag) return true;
			ExecuteNextCommand(command);

			return waitTimer > 0;
		}

		private void ExecuteNextCommand(BaseSequenceCommand cmd) {
			throw new NotImplementedException();
		}

		private void ExecuteNextCommand(RestCommand cmd) {
			waitTimer = cmd.Rest;
		}

		private void ExecuteNextCommand(ProgramChangeCommand cmd) {
			instrument = sequencePlayer.bank.instruments[(int)cmd.Program];
		}

		private void ExecuteNextCommand(NoteCommand cmd) {
			if(noteWait) {
				waitTimer = cmd.Duration;
			}
			if(tieMode) {
				NoteOnTie(cmd.Note, cmd.Velocity);
			} else {
				NoteOn(cmd.Note, cmd.Velocity, cmd.Duration);
			}
		}
		private void ExecuteNextCommand(NoteRandCommand cmd) {
			uint duration = (uint)Rand(cmd.DurationMin, cmd.DurationMax);
			if(noteWait) {
				waitTimer = duration;
			}
			if(tieMode) {
				NoteOnTie(cmd.Note, cmd.Velocity);
			} else {
				NoteOn(cmd.Note, cmd.Velocity, duration);
			}
		}
		private void ExecuteNextCommand(NoteVarCommand cmd) {
			uint duration = (uint)Var(cmd.DurationVar);
			if(noteWait) {
				waitTimer = duration;
			}
			if(tieMode) {
				NoteOnTie(cmd.Note, cmd.Velocity);
			} else {
				NoteOn(cmd.Note, cmd.Velocity, duration);
			}
		}

		private void ExecuteNextCommand(AllocateTracksCommand cmd) {
			sequencePlayer.tracks = new TrackPlayer[16];
			sequencePlayer.tracks[0] = this;
		}

		private void ExecuteNextComand(OpenTrackCommand cmd) {
			sequencePlayer.tracks[cmd.Track] = new TrackPlayer(sequencePlayer, cmd.target);
		}

		private void ExecuteNextCommand(TempoCommand cmd) {
			sequencePlayer.tempo = cmd.Tempo;
		}

		private void ExecuteNextCommand(EndTrackCommand cmd) {
			endFlag = true;
		}

		private void ExecuteNextCommand(JumpCommand cmd) {
			if(cmd.type == JumpCommand.JumpType.CALL) {
				callStack.Push(nextInstructionId);
			}
			nextInstructionId = cmd.target;
		}

		private void ExecuteNextCommand(VolumeCommand cmd) {
			if(cmd.Master) {
				sequencePlayer.MasterVolume = cmd.Volume;
			} else {
				Volume = cmd.Volume;
			}
		}
		private void ExecuteNextCommand(VolumeRandCommand cmd) {
			if(cmd.Master) {
				sequencePlayer.MasterVolume = (byte)Rand(cmd.VolumeMin, cmd.VolumeMax);
			} else {
				Volume = (byte)Rand(cmd.VolumeMin, cmd.VolumeMax);
			}
		}
		private void ExecuteNextCommand(VolumeVarCommand cmd) {
			if(cmd.Master) {
				sequencePlayer.MasterVolume = (byte)Var(cmd.VolumeVar);
			} else {
				Volume = (byte)Var(cmd.VolumeVar);
			}
		}

		private void ExecuteNextCommand(ExpressionCommand cmd) {
			Expression = cmd.Value;
		}
		private void ExecuteNextCommand(ExpressionRandCommand cmd) {
			Expression = (byte)Rand(cmd.Min, cmd.Max);
		}
		private void ExecuteNextCommand(ExpressionVarCommand cmd) {
			Expression = (byte)Var(cmd.Var);
		}

		private void ExecuteNextCommand(PanCommand cmd) {
			Pan(cmd.Pan);
		}
		private void ExecuteNextCommand(PanVarCommand cmd) {
			Pan((byte)Var(cmd.PanVar));
		}
		private void ExecuteNextCommand(PanRandCommand cmd) {
			Pan((byte)Rand(cmd.PanMin, cmd.PanMax));
		}

		private void ExecuteNextCommand(PitchBendCommand cmd) {
			if(cmd.IsRange) {
				PitchBendRange = cmd.Bend;
			} else {
				PitchBend = cmd.Bend;
			}
		}
		private void ExecuteNextCommand(PitchBendRandCommand cmd) {
			if(cmd.IsRange) {
				PitchBendRange = (byte)Rand(cmd.BendMin,cmd.BendMax);
			} else {
				PitchBend = (byte)Rand(cmd.BendMin, cmd.BendMax);
			}
		}
		private void ExecuteNextCommand(PitchBendVarCommand cmd) {
			if(cmd.IsRange) {
				PitchBendRange = (byte)Var(cmd.BendVar);
			} else {
				PitchBend = (byte)Var(cmd.BendVar);
			}
		}

		private void ExecuteNextCommand(MonoPolyCommand cmd) {
			noteWait = cmd.IsMono;
		}

		private void ExecuteNextCommand(TieCommand cmd) {
			tieMode = cmd.Tie;
			ReleaseAllNotes();
		}

		private void ExecuteNextCommand(PriorityCommand cmd) {
			this.Prio = cmd.Priority;
		}

		private void ExecuteNextCommand(LoopStartCommand cmd) {
			loopStack.Push(new LoopEntry(cmd.LoopCount, nextInstructionId));
		}

		private void ExecuteNextCommand(LoopEndCommand cmd) {
			var entry = loopStack.Peek();
			if(entry.loopCounter == 0) {
				loopStack.Pop();
			} else {
				entry.loopCounter--;
			}
		}

		private void ExecuteNextCommand(ReturnCommand cmd) {
			nextInstructionId = callStack.Pop();
		}

		private void ExecuteNextCommand(VarCommand cmd) {
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
		private void ExecuteNextCommand(VarVarCommand cmd) {
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
		private void ExecuteNextCommand(VarRandCommand cmd) {
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


		private class LoopEntry {
			public int loopCounter;
			public uint loopStart;

			public LoopEntry(int loopCount, uint loopStart) {
				loopCounter = loopCount;
				this.loopStart = loopStart;
			}
		}
	}
}