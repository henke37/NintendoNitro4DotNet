using System;
using System.Collections.Generic;
using System.Text;
using HenkesUtils;
using NitroComposer.SequenceCommands;

namespace NitroComposer {
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
            sb.Append(noteNames[noteVal]);
            if(oct == -1) {
                sb.Append("m1");
            } else {
                sb.Append(oct);
            }

            sb.Append(" ");
            sb.Append(cmd.Velocity);
            sb.Append(", ");
            sb.Append(cmd.Duration);
            sb.AppendLine();
        }

        private void Serialize(RestCommand cmd) {
            sb.Append("wait ");
            sb.Append(cmd.Rest);
            sb.AppendLine();
        }

        private void Serialize(EndTrackCommand cmd) {
            sb.AppendLine("fin");
        }

        private void Serialize(ProgramChangeCommand cmd) {
            sb.Append("prg ");
            sb.Append(cmd.Program);
            sb.AppendLine();
        }

        private void Serialize(AllocateTracksCommand cmd) {
            sb.AppendFormat("alloctrack {0:X4}\n",cmd.Tracks);
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
            if(track<0) {
                if(track == -2) {
                    return "S" + position;
                } else {
                    return "L" + position;
                }
            }
            return "T"+track;
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

        private void Serialize(PanCommand cmd) {
            sb.AppendFormat("pan {0}\n", cmd.Pan);
        }

        private void Serialize(ReturnCommand cmd) {
            sb.AppendLine("ret");
        }

        private void Serialize(PitchBendCommand cmd) {
            if(cmd.IsRange) {
                sb.AppendFormat("bendrange {0}\n", cmd.Bend);
            } else {
                sb.AppendFormat("pitchbend {0}\n", cmd.Bend);
            }
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

        private void Serialize(ExpressionCommand cmd) {
            sb.AppendFormat("volume2 {0}\n", cmd.Value);
        }

        private void Serialize(PriorityCommand cmd) {
            sb.AppendFormat("prio {0}\n", cmd.Priority);
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
    }
}
