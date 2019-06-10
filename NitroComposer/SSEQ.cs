using System.IO;
using Henke37.IOUtils;
using Nitro;

namespace Henke37.Nitro.Composer {
	public class SSEQ {
        public Sequence sequence;

        public SSEQ(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SSEQ");

            //readHEAD(sections.Open("HEAD"));
            ParseData(sections);
        }

        private void ParseData(SectionedFile sections) {
            var section = sections.FindSection("DATA");
            var sectionStream = sections.Open(section);
            BinaryReader reader = new BinaryReader(sectionStream);

            //for some inane reason it's prefixed with the offset into the file for the data itself
            long offset=reader.ReadUInt32();
            offset -= section.position;

            var commandStream = new SubStream(sectionStream, offset);

            var parser = new SequenceParser(commandStream);
            sequence = parser.Parse();
        }
    }
}