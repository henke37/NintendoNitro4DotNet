using Henke37.IOUtils;
using System;
using System.IO;

namespace Henke37.Nitro.Composer {
	public class Wave {

        public WaveEncoding Encoding;

        public bool Loops;
        public UInt16 LoopStart;
        public uint LoopLength;

        public UInt16 SampleRate;
        public UInt16 TimerLen;

        public Stream dataStream;


        public Wave(Stream stream) {
            using(var r=new BinaryReader(stream)) {
                Encoding = (WaveEncoding)r.ReadByte();
                Loops = r.ReadBoolean();
                SampleRate = r.ReadUInt16();
                TimerLen = r.ReadUInt16();
                LoopStart = r.ReadUInt16();
                LoopLength = r.ReadUInt32();
            }
            dataStream = new SubStream(stream, stream.Position);
        }
    }
}