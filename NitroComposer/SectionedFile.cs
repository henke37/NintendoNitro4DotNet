using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HenkesUtils;

namespace NitroComposer {
    internal class SectionedFile {
        private Stream mainStream;

        private Dictionary<string, SectionEntry> sections;

        public string MainId { get; }

        public SectionedFile(Stream mainStream, string expectedSignature=null) {
            this.mainStream = mainStream;;
            using(var r=new BinaryReader(mainStream,Encoding.UTF8,true)) {
                MainId = r.Read4C();

                if(expectedSignature != null && MainId != expectedSignature) throw new InvalidDataException("Signature mismatch!");

                var magic = r.ReadUInt32();

                var sectionSize = r.ReadUInt32();

                var headerSize = r.ReadUInt16();
                if(headerSize != 0x10) throw new InvalidDataException("Headersize has to be 0x10");

                LoadSections(mainStream, r);
            }
        }

        private void LoadSections(Stream mainStream, BinaryReader r) {
            int sectionCount = r.ReadUInt16();
            sections = new Dictionary<string, SectionEntry>((int)sectionCount);

            for(int sectionIndex = 0; sectionIndex < sectionCount; ++sectionIndex) {
                var section = new SectionEntry();
                section.name = r.Read4C();
                section.size = r.ReadUInt32();
                section.position = mainStream.Position;
                r.Skip((int)section.size - 8);
                sections.Add(section.name, section);
            }
        }

        internal Stream Open(string name) {
            var section = sections[name];
            return new SubStream(mainStream, section.position, section.size);
        }

        private class SectionEntry {
            public string name;
            public long position;
            public UInt32 size;
        }
    }
}