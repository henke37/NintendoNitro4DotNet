using Henke37.IOUtils;
using Henke37.Nitro;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Composer {
	public class SWAR {

        public List<Wave> waves;

        public SWAR(Stream mainStream) {
            var sections = new SectionedFile(mainStream, "SWAR");

            //readHEAD(sections.Open("HEAD"));
            parseData(sections);
        }

        private void parseData(SectionedFile sections) {
            var section = sections["DATA"];
            var baseStream = sections.Open(section);

            using(var r = new BinaryReader(new SubStream(baseStream,0))) {
                r.Skip(8 * 4);
                uint waveCount = r.ReadUInt32();

                waves = new List<Wave>((int)waveCount);

                for(uint waveIndex=0;waveIndex<waveCount;++waveIndex) {
                    long offset = r.ReadUInt32();
                    //for some inane reason the pointer is relative to the start of the file
                    offset -= section.position;
                    var wave = new Wave(new SubStream(baseStream,offset));
                    waves.Add(wave);
                }
            }
        }
    }
}