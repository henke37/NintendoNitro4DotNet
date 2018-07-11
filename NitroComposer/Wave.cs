using HenkesUtils;
using System;
using System.IO;

namespace Nitro.Composer {
    public class Wave {

        public WaveEncoding Encoding;

        public bool Loops;
        public UInt16 LoopStart;
        public uint NonLoopLength;

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
                NonLoopLength = r.ReadUInt32();
            }
            dataStream = new SubStream(stream, stream.Position);
        }

        public enum WaveEncoding : byte {
            PCM8,
            PCM16,
            ADPCM,
            GEN
        }
    }
}