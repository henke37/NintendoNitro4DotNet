using System;
using System.IO;
using HenkesUtils;

namespace NitroComposer {
    public class SSEQ {
        public Sequence sequence;

        public SSEQ(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SSEQ");

            //readHEAD(sections.Open("HEAD"));
            ParseData(sections);
        }

        private void ParseData(SectionedFile sections) {
            BinaryReader reader = new BinaryReader(sections.Open("DATA"));

            //for some inane reason it's prefixed with the offset into the file for the data itself
            reader.Skip(4);

            var parser = new SequenceDisassembler(reader);
            sequence = parser.Parse();
        }
    }
}