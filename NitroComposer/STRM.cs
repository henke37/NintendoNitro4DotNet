using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HenkesUtils;

namespace NitroComposer {
    public class STRM {
        public byte encoding;
        public bool loop;
        public byte channels;
        public ushort sampleRate;
        public uint loopPoint;
        public uint sampleCount;
        public uint dataPos;
        public uint nBlock;
        public uint blockLength;
        public uint blockSamples;
        public uint lastBlockLength;
        public uint lastBlockSamples;

        public STRM(Stream mainStream) {
            var sections = new SectionedFile(mainStream);
            if(sections.MainId != "STRM") throw new InvalidDataException("Invalid STRM block, wrong type id");

            readHEAD(sections.Open("HEAD"));
        }

        private void readHEAD(Stream stream) {
            using(var r=new BinaryReader(stream)) {
                encoding = r.ReadByte();
                loop = r.ReadBoolean();
                channels = r.ReadByte();
                r.Skip(1);
                sampleRate = r.ReadUInt16();
                r.Skip(2);
                loopPoint = r.ReadUInt32();
                sampleCount = r.ReadUInt32();
                dataPos = r.ReadUInt32();
                nBlock = r.ReadUInt32();
                blockLength = r.ReadUInt32();
                blockSamples = r.ReadUInt32();
                lastBlockLength = r.ReadUInt32();
                lastBlockSamples = r.ReadUInt32();
            }
        }

        public float PlaybackLength => sampleCount / ((float)sampleRate);
    }
}
