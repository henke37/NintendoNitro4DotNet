using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroComposer {
    public class STRM {
        public STRM(Stream mainStream) {
            var sections = new SectionedFile(mainStream);
            if(sections.MainId != "STRM") throw new InvalidDataException("Invalid STRM block, wrong type id");

            readHEAD(sections.Open("HEAD"));
        }

        private void readHEAD(Stream stream) {
            using(var r=new BinaryReader(stream)) {

            }
        }
    }
}
