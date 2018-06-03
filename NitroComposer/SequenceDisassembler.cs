using NitroComposer.SequenceCommands;
using System;
using System.Collections.Generic;
using System.IO;

namespace NitroComposer {
    class SequenceDisassembler {
        private Dictionary<uint, Flow> flows;
        private Stack<Flow> unparsedFlows;
        private uint commandIndex;

        private BinaryReader reader;

        public SequenceDisassembler(BinaryReader reader) {
            this.reader = reader;
            flows = new Dictionary<uint, Flow>();
            unparsedFlows = new Stack<Flow>();
        }

        public Sequence Parse() {
            Sequence seq=new Sequence();

            AddOrFindFlow(0);

            while(unparsedFlows.Count>0) {
                var flow = unparsedFlows.Pop();
                ParseFlow(flow);
            }

            return seq;
        }

        private void ParseFlow(Flow flow) {
            if(flow.parsed) throw new Exception("Already parsed this flow!");
            flow.parsed = true;
            flow.commandIndex = commandIndex;

            reader.BaseStream.Position = flow.offset;

            for(; ;) {
                BaseSequenceCommand cmd = readCommand();
                commandIndex++;
            }
        }

        private BaseSequenceCommand readCommand() {
            uint id = reader.ReadByte();
            if(id<0x80) {
                return new NoteCommand(id, reader.ReadByte(),reader.ReadMIDIVarLen());
            }
            switch(id) {
                case 0x80:
                    return new RestCommand(reader.ReadMIDIVarLen());

                case 0x81:
                    return new ProgramChangeCommand(reader.ReadMIDIVarLen());

                case 0x93: {
                    byte trackId = reader.ReadByte();
                    uint trackOffset = reader.Read3ByteInt();
                    AddOrFindFlow(trackOffset);
                    return new OpenTrackCommand(trackId,trackOffset);
                }

                case 0x94: {
                    uint jumpOffset = reader.Read3ByteInt();
                    AddOrFindFlow(jumpOffset);
                    return new JumpCommand(jumpOffset,JumpCommand.JumpType.JUMP);
                }

                case 0x95: {
                    uint jumpOffset = reader.Read3ByteInt();
                    AddOrFindFlow(jumpOffset);
                    return new JumpCommand(jumpOffset, JumpCommand.JumpType.CALL);
                }

                case 0xA2:
                    return readIfCommand();

                case 0xB0:
                case 0xB1:
                case 0xB2:
                case 0xB3:
                case 0xB4:
                case 0xB5:
                case 0xB6:
                case 0xB8:
                case 0xB9:
                case 0xBA:
                case 0xBB:
                case 0xBC:
                case 0xBD:
                    return new VarCommand((VarCommand.Operator)id, reader.ReadByte(), reader.Read3ByteInt());

                case 0xC0:
                    return new PanCommand(reader.ReadByte());
                case 0xC1:
                    return new VolumeCommand(reader.ReadByte(), false);
                case 0xC2:
                    return new VolumeCommand(reader.ReadByte(), true);
                case 0xC3:
                    return new TransposeCommand(reader.ReadByte());
                case 0xC4:
                    return new PitchBendCommand(reader.ReadByte(), false);
                case 0xC5:
                    return new PitchBendCommand(reader.ReadByte(), true);
                case 0xC6:
                    return new PriorityCommand(reader.ReadByte());
                case 0xC7:
                    return new MonoPolyCommand(reader.ReadBoolean());
                case 0xC8:
                    return new TieCommand(reader.ReadBoolean());
                case 0xC9:
                    return new PortamentoKeyCommand(reader.ReadByte());

                case 0xCA:
                case 0xCB:
                case 0xCC:
                case 0xCD:
                    return new ModulationCommand((ModulationCommand.ModType)id,reader.ReadByte());

                case 0xCE:
                    return new PortamentoCommand(reader.ReadBoolean());
                case 0xCF:
                    return new PortamentoTimeCommand(reader.ReadByte());

                case 0xD0:
                case 0xD1:
                case 0xD2:
                case 0xD3:
                    return new ADSRCommand((ADSRCommand.EnvPos)id, reader.ReadByte());

                case 0xD4:
                    return new LoopStartCommand(reader.ReadByte());
                case 0xD5:
                    return new ExpressionCommand(reader.ReadByte());
                case 0xD6:
                    return new PrintVariableCommand(reader.ReadByte());
                case 0xE0:
                    return new ModulationDelayCommand(reader.ReadUInt16());
                case 0xE1:
                    return new TempoCommand(reader.ReadUInt16());
                case 0xE3:
                    return new SweepPitchCommand(reader.ReadUInt16());
                case 0xFC:
                    return new LoopEndCommand();
                case 0xFD:
                    return new ReturnCommand();
                case 0xFE:
                    return new AllocateTracksCommand(reader.ReadUInt16());
                case 0xFF:
                    return new EndTrackCommand();
                default:
                    throw new InvalidDataException("Unknown command");
            }
        }

        private BaseSequenceCommand readIfCommand() {
            throw new NotImplementedException();
        }

        private Flow AddOrFindFlow(uint offset) {
            Flow flow;
            if(flows.TryGetValue(offset, out flow)) return flow;

            flow= new Flow(offset);
            flows.Add(offset, flow);
            unparsedFlows.Push(flow);
            return flow;
        }

        private class Flow {
            public bool parsed;
            public uint offset;
            public uint commandIndex;

            public Flow(uint offset) {
                this.offset = offset;
            }
        }
    }
}
