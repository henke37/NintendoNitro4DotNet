using System.IO;

namespace NitroComposer {
    public class SWAR {
        public SWAR(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SWAR");

            //readHEAD(sections.Open("HEAD"));
        }
    }
}