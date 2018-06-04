using HenkesUtils;
using NitroComposer.Instruments;
using System;
using System.Collections.Generic;
using System.IO;

namespace NitroComposer {
    public class SBNK {

        public List<Instrument> instruments;

        public SBNK(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SBNK");

            //readHEAD(sections.Open("HEAD"));
            parseData(sections);
        }

        private void parseData(SectionedFile sections) {
            uint instrumentCount;

            var section = sections.FindSection("DATA");
            var baseStream = sections.Open(section);

            using(var r = new BinaryReader(new SubStream(baseStream, 0))) {
                instrumentCount = r.ReadUInt32();

                instruments = new List<Instrument>((int)instrumentCount);
                for(int instrumentIndex = 0; instrumentIndex < instrumentCount; ++instrumentIndex) {
                    byte instrumentType=r.ReadByte();
                    long offset = r.Read3ByteUInt();
                    //for some inane reason, everything is offset from the start of the file, not the section
                    offset -= section.position;

                    var instrument = Instrument.parseRecord(instrumentType, new SubStream(baseStream, offset));
                }
            }
        }
    }
}