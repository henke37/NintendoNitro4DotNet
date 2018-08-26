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

		private uint waitTimer;

		private bool conditionFlag=false;

		private Instrument instrument;

		private Stack<uint> callStack=new Stack<uint>();
		private Stack<LoopEntry> loopStack=new Stack<LoopEntry>();

		public TrackPlayer(SequencePlayer sequencePlayer, uint nextInstructionId=0) {
			this.sequencePlayer = sequencePlayer;
			this.nextInstructionId = nextInstructionId;

			instrument = sequencePlayer.bank.instruments[0];
		}

		private bool NoteOn(uint note, uint velocity, uint duration) {
			throw new NotImplementedException();
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

		/** Returns true if another command should be executed right away and false if there is a delay to wait out */
		internal bool ExecuteNextCommand() {
			dynamic command = sequencePlayer.sseq.sequence.commands[(int)nextInstructionId++];
			if(command.Conditional && !conditionFlag) return true;
			return ExecuteNextCommand(command);
		}

		private bool ExecuteNextCommand(BaseSequenceCommand cmd) {
			throw new NotImplementedException();
		}

		private bool ExecuteNextCommand(RestCommand cmd) {
			waitTimer = cmd.Rest;
			return false;
		}

		private bool ExecuteNextCommand(ProgramChangeCommand cmd) {
			instrument = sequencePlayer.bank.instruments[(int)cmd.Program];
			return true;
		}

		private bool ExecuteNextCommand(NoteCommand cmd) {
			return NoteOn(cmd.Note, cmd.Velocity, cmd.Duration);
		}
		private bool ExecuteNextCommand(NoteRandCommand cmd) {
			return NoteOn(cmd.Note, cmd.Velocity, (uint)Rand(cmd.DurationMin, cmd.DurationMax));
		}
		private bool ExecuteNextCommand(NoteVarCommand cmd) {
			return NoteOn(cmd.Note, cmd.Velocity, (uint)Var(cmd.DurationVar));
		}

		private bool ExecuteNextCommand(AllocateTracksCommand cmd) {
			sequencePlayer.tracks = new TrackPlayer[16];
			sequencePlayer.tracks[0] = this;
			return true;
		}

		private bool ExecuteNextComand(OpenTrackCommand cmd) {
			sequencePlayer.tracks[cmd.Track] = new TrackPlayer(sequencePlayer, cmd.target);
			return true;
		}

		private bool ExecuteNextCommand(TempoCommand cmd) {
			sequencePlayer.tempo = cmd.Tempo;
			return true;
		}

		private bool ExecuteNextCommand(EndTrackCommand cmd) {
			endFlag = true;
			return true;
		}

		private bool ExecuteNextCommand(JumpCommand cmd) {
			if(cmd.type == JumpCommand.JumpType.CALL) {
				callStack.Push(nextInstructionId);
			}
			nextInstructionId = cmd.target;
			return true;
		}

		private bool ExecuteNextCommand(PanCommand cmd) {
			Pan(cmd.Pan);
			return true;
		}
		private bool ExecuteNextCommand(PanVarCommand cmd) {
			Pan((byte)Var(cmd.PanVar));
			return true;
		}
		private bool ExecuteNextCommand(PanRandCommand cmd) {
			Pan((byte)Rand(cmd.PanMin,cmd.PanMax));
			return true;
		}

		private bool ExecuteNextCommand(MonoPolyCommand cmd) {
			noteWait = cmd.IsMono;
			return true;
		}

		private bool ExecuteNextCommand(LoopStartCommand cmd) {
			loopStack.Push(new LoopEntry(cmd.LoopCount, nextInstructionId));
			return true;
		}

		private bool ExecuteNextCommand(LoopEndCommand cmd) {
			var entry = loopStack.Peek();
			if(entry.loopCounter == 0) {
				loopStack.Pop();
				return true;
			} else {
				entry.loopCounter--;
				return true;
			}
		}

		private bool ExecuteNextCommand(ReturnCommand cmd) {
			nextInstructionId = callStack.Pop();
			return true;
		}

		private bool ExecuteNextCommand(VarCommand cmd) {
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
					if(cmd.Operand<0) {
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
			return true;
		}
		private bool ExecuteNextCommand(VarVarCommand cmd) {
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
			return true;
		}
		private bool ExecuteNextCommand(VarRandCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ADD:
					SetVar(cmd.Variable, (short)(Var(cmd.Variable) + Rand(cmd.OperandMin,cmd.OperandMax)));
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
			return true;
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