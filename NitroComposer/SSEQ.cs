using System;
using System.IO;

namespace NitroComposer {
    public class SSEQ {
        public SSEQ(Stream mainStream) {
            var sections = new SectionedFile(mainStream);
            if(sections.MainId != "SSEQ") throw new InvalidDataException("Invalid SSEQ block, wrong type id");

            readHEAD(sections.Open("HEAD"));
        }

        private void readHEAD(Stream stream) {
            throw new NotImplementedException();
        }
    }
}