using System.IO;

namespace NitroComposer {
    public class SBNK {
        public SBNK(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SBNK");

            //readHEAD(sections.Open("HEAD"));
        }
    }
}