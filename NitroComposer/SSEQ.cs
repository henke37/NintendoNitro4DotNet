using System;
using System.IO;

namespace NitroComposer {
    public class SSEQ {
        public SSEQ(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SSEQ");

            //readHEAD(sections.Open("HEAD"));
        }
    }
}