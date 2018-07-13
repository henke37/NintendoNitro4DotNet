using System;
using System.Collections.Generic;
using System.Text;
using HenkesUtils;
using Nitro.Composer.SequenceCommands;
using Nitro.Composer.SequenceCommands.Rand;
using Nitro.Composer.SequenceCommands.Var;

namespace Nitro.Composer {
	public class SequenceSerializer {

		private StringBuilder sb;

		private Dictionary<uint, int> jumpTargets;

		public SequenceSerializer() {
			sb = new StringBuilder();
		}

		public string Serialize(Sequence seq) {
			BuildJumpTable(seq);

			foreach(dynamic cmd in seq.commands) {
				Serialize(cmd);
			}

			return sb.ToString();
		}

		private void BuildJumpTable(Sequence seq) {
			jumpTargets = new Dictionary<uint, int>();
			foreach(BaseSequenceCommand cmd in seq.commands) {
				JumpCommand jmp = cmd as JumpCommand;
				if(jmp == null) continue;
				if(jmp is OpenTrackCommand trk) {
					jumpTargets[jmp.target] = trk.Track;
				} else {
					if(jmp.type == JumpCommand.JumpType.CALL) {
						jumpTargets.TrySet(jmp.target, -2);
					} else {
						jumpTargets.TrySet(jmp.target, -1);
					}
				}
			}
		}

		private void Serialize(BaseSequenceCommand cmd) {
			throw new NotImplementedException();
		}

		private static readonly string[] noteNames = new string[] {
			"cn", "cs", "dn", "ds", "en", "fn",
			"fs", "gn", "gs", "an", "as", "bn"
		};

		private void Serialize(NoteCommand cmd) {
			byte noteVal = (byte)(cmd.Note % 12);
			sbyte oct = (sbyte)(cmd.Note / 12);
			oct--;
			if(oct == -1) {
				sb.AppendFormat("{0}m1 {1}, {2}\n", noteNames[noteVal], cmd.Velocity, cmd.Duration);
			} else {
				sb.AppendFormat("{0}{1} {2}, {3}\n", noteNames[noteVal], oct, cmd.Velocity, cmd.Duration);
			}
		}
		private void Serialize(NoteVarCommand cmd) {
			byte noteVal = (byte)(cmd.Note % 12);
			sbyte oct = (sbyte)(cmd.Note / 12);
			oct--;
			if(oct == -1) {
				sb.AppendFormat("{0}m1_v {1}, {2}\n", noteNames[noteVal], cmd.Velocity, cmd.DurationVar);
			} else {
				sb.AppendFormat("{0}{1}_v {2}, {3}\n", noteNames[noteVal], oct, cmd.Velocity, cmd.DurationVar);
			}
		}
		private void Serialize(NoteRandCommand cmd) {
			byte noteVal = (byte)(cmd.Note % 12);
			sbyte oct = (sbyte)(cmd.Note / 12);
			oct--;
			if(oct == -1) {
				sb.AppendFormat("{0}m1_r {1}, {2}, {3}\n", noteNames[noteVal], cmd.Velocity, cmd.DurationMin, cmd.DurationMax);
			} else {
				sb.AppendFormat("{0}{1}_r {2}, {3}, {4}\n", noteNames[noteVal], oct, cmd.Velocity, cmd.DurationMin, cmd.DurationMax);
			}
		}

		public void Clear() {
			sb.Clear();
		}

		private void Serialize(RestCommand cmd) {
			sb.AppendFormat("wait {0}\n", cmd.Rest);
		}
		private void Serialize(RestVarCommand cmd) {
			sb.AppendFormat("wait_v {0}\n", cmd.RestVar);
		}
		private void Serialize(RestRandCommand cmd) {
			sb.AppendFormat("wait_r {0}, {1}\n", cmd.RestMin, cmd.RestMax);
		}

		private void Serialize(EndTrackCommand cmd) {
			sb.AppendLine("fin");
		}

		private void Serialize(ProgramChangeCommand cmd) {
			sb.Append("prg ");
			sb.Append(cmd.Program);
			sb.AppendLine();
		}
		private void Serialize(ProgramChangeVarCommand cmd) {
			sb.AppendFormat("prg_v {0}\n", cmd.ProgramVar);
		}
		private void Serialize(ProgramChangeRandCommand cmd) {
			sb.AppendFormat("prg_r {0}, {1}\n", cmd.ProgramMin, cmd.ProgramMax);
		}

		private void Serialize(AllocateTracksCommand cmd) {
			sb.AppendFormat("alloctrack {0:X4}\n", cmd.Tracks);
		}

		private void Serialize(TempoCommand cmd) {
			sb.AppendFormat("tempo {0}\n", cmd.Tempo);
		}

		private void Serialize(MonoPolyCommand cmd) {
			if(cmd.IsMono) {
				sb.AppendLine("notewait_on");
			} else {
				sb.AppendLine("notewait_off");
			}
		}

		private string positionLabel(uint position) {
			int track = jumpTargets[position];
			if(track < 0) {
				if(track == -2) {
					return "S" + position;
				} else {
					return "L" + position;
				}
			}
			return "T" + track;
		}

		private void Serialize(LoopEndCommand cmd) {
			sb.AppendLine("loop_end");
		}

		private void Serialize(LoopStartCommand cmd) {
			sb.AppendFormat("loop_start {0}\n", cmd.LoopCount);
		}
		private void Serialize(LoopStartVarCommand cmd) {
			sb.AppendFormat("loop_start_v {0}\n", cmd.LoopCountVar);
		}

		private void Serialize(OpenTrackCommand cmd) {
			sb.AppendFormat("opentrack {0}, {1}\n", positionLabel(cmd.target), cmd.Track);
		}

		private void Serialize(JumpCommand cmd) {
			switch(cmd.type) {
				case JumpCommand.JumpType.JUMP:
					sb.AppendFormat("jump {0}\n", positionLabel(cmd.target));
					break;
				case JumpCommand.JumpType.CALL:
					sb.AppendFormat("call {0}\n", positionLabel(cmd.target));
					break;
				default:
					throw new ArgumentException("bogus jump type");
			}
		}

		private void Serialize(VolumeCommand cmd) {
			if(cmd.Master) {
				sb.AppendFormat("main_volume {0}\n", cmd.Volume);
			} else {
				sb.AppendFormat("volume {0}\n", cmd.Volume);
			}
		}
		private void Serialize(VolumeVarCommand cmd) {
			if(cmd.Master) {
				sb.AppendFormat("main_volume_v {0}\n", cmd.VolumeVar);
			} else {
				sb.AppendFormat("volume_v {0}\n", cmd.VolumeVar);
			}
		}
		private void Serialize(VolumeRandCommand cmd) {
			if(cmd.Master) {
				sb.AppendFormat("main_volume_r {0}, {1}\n", cmd.VolumeMin, cmd.VolumeMax);
			} else {
				sb.AppendFormat("volume_r {0}, {1}\n", cmd.VolumeMin, cmd.VolumeMax);
			}
		}

		private void Serialize(PanCommand cmd) {
			sb.AppendFormat("pan {0}\n", cmd.Pan);
		}
		private void Serialize(PanVarCommand cmd) {
			sb.AppendFormat("pan_v {0}\n", cmd.PanVar);
		}
		private void Serialize(PanRandCommand cmd) {
			sb.AppendFormat("pan_r {0}, {1}\n", cmd.PanMin, cmd.PanMax);
		}

		private void Serialize(TransposeCommand cmd) {
			sb.AppendFormat("transpose {0}\n", cmd.Transpose);
		}
		private void Serialize(TransposeVarCommand cmd) {
			sb.AppendFormat("transpose_v {0}\n", cmd.TransposeVar);
		}
		private void Serialize(TransposeRandCommand cmd) {
			sb.AppendFormat("transpose_r {0}, {1}\n", cmd.TransposeMin, cmd.TransposeMax);
		}

		private void Serialize(ReturnCommand cmd) {
			sb.AppendLine("ret");
		}

		private void Serialize(TieCommand cmd) {
			if(cmd.Tie) {
				sb.AppendLine("tieon");
			} else {
				sb.AppendLine("tieoff");
			}
		}

		private void Serialize(PitchBendCommand cmd) {
			if(cmd.IsRange) {
				sb.AppendFormat("bendrange {0}\n", cmd.Bend);
			} else {
				sb.AppendFormat("pitchbend {0}\n", cmd.Bend);
			}
		}
		private void Serialize(PitchBendVarCommand cmd) {
			if(cmd.IsRange) {
				sb.AppendFormat("bendrange_v {0}\n", cmd.BendVar);
			} else {
				sb.AppendFormat("pitchbend_v {0}\n", cmd.BendVar);
			}
		}
		private void Serialize(PitchBendRandCommand cmd) {
			if(cmd.IsRange) {
				sb.AppendFormat("bendrange_r {0}, {1}\n", cmd.BendMin, cmd.BendMax);
			} else {
				sb.AppendFormat("pitchbend_r {0}, {1}\n", cmd.BendMin, cmd.BendMax);
			}
		}

		private void Serialize(SweepPitchCommand cmd) {
			sb.AppendFormat("sweep_pitch {0}\n", cmd.Ammount);
		}
		private void Serialize(SweepPitchRandCommand cmd) {
			sb.AppendFormat("sweep_pitch_r {0}, {1}\n", cmd.AmmountMin, cmd.AmmountMax);
		}

		private void Serialize(ModulationCommand cmd) {
			switch(cmd.Type) {
				case ModulationCommand.ModType.DEPTH:
					sb.AppendFormat("mod_depth {0}\n", cmd.Value);
					break;
				case ModulationCommand.ModType.RANGE:
					sb.AppendFormat("mod_range {0}\n", cmd.Value);
					break;
				case ModulationCommand.ModType.SPEED:
					sb.AppendFormat("mod_speed {0}\n", cmd.Value);
					break;
				case ModulationCommand.ModType.TYPE:
					sb.AppendFormat("mod_type {0}\n", cmd.Value);
					break;

			}
		}
		private void Serialize(ModulationVarCommand cmd) {
			switch(cmd.Type) {
				case ModulationCommand.ModType.DEPTH:
					sb.AppendFormat("mod_depth_v {0}\n", cmd.Var);
					break;
				case ModulationCommand.ModType.RANGE:
					sb.AppendFormat("mod_range_v {0}\n", cmd.Var);
					break;
				case ModulationCommand.ModType.SPEED:
					sb.AppendFormat("mod_speed_v {0}\n", cmd.Var);
					break;
				case ModulationCommand.ModType.TYPE:
					sb.AppendFormat("mod_type_v {0}\n", cmd.Var);
					break;

			}
		}
		private void Serialize(ModulationRandCommand cmd) {
			switch(cmd.Type) {
				case ModulationCommand.ModType.DEPTH:
					sb.AppendFormat("mod_depth_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
				case ModulationCommand.ModType.RANGE:
					sb.AppendFormat("mod_range_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
				case ModulationCommand.ModType.SPEED:
					sb.AppendFormat("mod_speed_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
				case ModulationCommand.ModType.TYPE:
					sb.AppendFormat("mod_type_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;

			}
		}

		private void Serialize(ModulationDelayCommand cmd) {
			sb.AppendFormat("mod_delay {0}\n", cmd.Delay);
		}

		private void Serialize(ADSRCommand cmd) {
			switch(cmd.envPos) {
				case ADSRCommand.EnvPos.ATTACK:
					sb.AppendFormat("attack {0}\n", cmd.Value);
					break;
				case ADSRCommand.EnvPos.DECAY:
					sb.AppendFormat("decay {0}\n", cmd.Value);
					break;
				case ADSRCommand.EnvPos.SUSTAIN:
					sb.AppendFormat("sustain {0}\n", cmd.Value);
					break;
				case ADSRCommand.EnvPos.RELEASE:
					sb.AppendFormat("release {0}\n", cmd.Value);
					break;
			}
		}
		private void Serialize(ADSRVarCommand cmd) {
			switch(cmd.envPos) {
				case ADSRCommand.EnvPos.ATTACK:
					sb.AppendFormat("attack_v {0}\n", cmd.Var);
					break;
				case ADSRCommand.EnvPos.DECAY:
					sb.AppendFormat("decay_v {0}\n", cmd.Var);
					break;
				case ADSRCommand.EnvPos.SUSTAIN:
					sb.AppendFormat("sustain_v {0}\n", cmd.Var);
					break;
				case ADSRCommand.EnvPos.RELEASE:
					sb.AppendFormat("release_v {0}\n", cmd.Var);
					break;
			}
		}
		private void Serialize(ADSRRandCommand cmd) {
			switch(cmd.envPos) {
				case ADSRCommand.EnvPos.ATTACK:
					sb.AppendFormat("attack_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
				case ADSRCommand.EnvPos.DECAY:
					sb.AppendFormat("decay_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
				case ADSRCommand.EnvPos.SUSTAIN:
					sb.AppendFormat("sustain_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
				case ADSRCommand.EnvPos.RELEASE:
					sb.AppendFormat("release_r {0}, {1}\n", cmd.Min, cmd.Max);
					break;
			}
		}

		private void Serialize(ExpressionCommand cmd) {
			sb.AppendFormat("volume2 {0}\n", cmd.Value);
		}
		private void Serialize(ExpressionVarCommand cmd) {
			sb.AppendFormat("volume2_v {0}\n", cmd.Var);
		}
		private void Serialize(ExpressionRandCommand cmd) {
			sb.AppendFormat("volume2_r {0}, {1}\n", cmd.Min, cmd.Max);
		}

		private void Serialize(PriorityCommand cmd) {
			sb.AppendFormat("prio {0}\n", cmd.Priority);
		}

		private void Serialize(PortamentoTimeCommand cmd) {
			sb.AppendFormat("porta_time {0}\n", cmd.Time);
		}
		private void Serialize(PortamentoTimeVarCommand cmd) {
			sb.AppendFormat("porta_time_v {0}\n", cmd.TimeVar);
		}
		private void Serialize(PortamentoTimeRandCommand cmd) {
			sb.AppendFormat("porta_time_r {0}, {1}\n", cmd.TimeMin, cmd.TimeMax);
		}

		private void Serialize(PortamentoKeyCommand cmd) {
			sb.AppendFormat("porta {0}\n", cmd.Key);
		}

		private void Serialize(PortamentoCommand cmd) {
			if(cmd.Enable) {
				sb.AppendLine("porta_on");
			} else {
				sb.AppendLine("porta_off");
			}
		}

		private void Serialize(VarCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ASSIGN:
					sb.AppendFormat("setvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.ADD:
					sb.AppendFormat("addvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.SUB:
					sb.AppendFormat("subvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.MUL:
					sb.AppendFormat("mulvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.DIV:
					sb.AppendFormat("divvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.SHIFT:
					sb.AppendFormat("shiftvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.EQU:
					sb.AppendFormat("cmp_eq {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.GT:
					sb.AppendFormat("cmp_gt {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.GTE:
					sb.AppendFormat("cmp_ge {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.LTE:
					sb.AppendFormat("cmp_le {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.LT:
					sb.AppendFormat("cmp_lt {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.NEQ:
					sb.AppendFormat("cmp_ne {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				case VarCommand.Operator.RAND:
					sb.AppendFormat("randvar {0}, {1}\n", cmd.Variable, cmd.Operand);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void Serialize(VarVarCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ASSIGN:
					sb.AppendFormat("setvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.ADD:
					sb.AppendFormat("addvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.SUB:
					sb.AppendFormat("subvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.MUL:
					sb.AppendFormat("mulvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.DIV:
					sb.AppendFormat("divvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.SHIFT:
					sb.AppendFormat("shiftvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.EQU:
					sb.AppendFormat("cmp_eq_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.GT:
					sb.AppendFormat("cmp_gt_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.GTE:
					sb.AppendFormat("cmp_ge_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.LTE:
					sb.AppendFormat("cmp_le_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.LT:
					sb.AppendFormat("cmp_lt_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.NEQ:
					sb.AppendFormat("cmp_ne_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				case VarCommand.Operator.RAND:
					sb.AppendFormat("randvar_v {0}, {1}\n", cmd.Variable1, cmd.Variable2);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void Serialize(VarRandCommand cmd) {
			switch(cmd.Op) {
				case VarCommand.Operator.ASSIGN:
					sb.AppendFormat("setvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.ADD:
					sb.AppendFormat("addvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.SUB:
					sb.AppendFormat("subvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.MUL:
					sb.AppendFormat("mulvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.DIV:
					sb.AppendFormat("divvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.SHIFT:
					sb.AppendFormat("shiftvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.EQU:
					sb.AppendFormat("cmp_eq_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.GT:
					sb.AppendFormat("cmp_gt_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.GTE:
					sb.AppendFormat("cmp_ge_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.LTE:
					sb.AppendFormat("cmp_le_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.LT:
					sb.AppendFormat("cmp_lt_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.NEQ:
					sb.AppendFormat("cmp_ne_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				case VarCommand.Operator.RAND:
					sb.AppendFormat("randvar_r {0}, {1}, {2}\n", cmd.Variable, cmd.OperandMin, cmd.OperandMax);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
