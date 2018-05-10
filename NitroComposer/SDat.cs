using HenkesUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroComposer {
    public class SDat {
        private static readonly byte[] SIGNATURE_SDAT= { (byte)'S', (byte)'D', (byte)'A' , (byte)'T' };
        private static readonly byte[] SIGNATURE_FAT = { (byte)'F', (byte)'A', (byte)'T', (byte)' ' };

        public static SDat Open(string filename) {
            return Open(File.OpenRead(filename));
        }

        public static SDat Open(Stream stream) {
            var sdat = new SDat();
            sdat.Parse(stream);
            return sdat;
        }

        private List<FATRecord> files;
        private Stream mainStream;

        public SDat() {

        }

        private void Parse(Stream stream) {
            if(!stream.CanRead) throw new ArgumentException("Stream must be readable!", nameof(stream));
            if(!stream.CanSeek) throw new ArgumentException("Stream must be seekable!", nameof(stream));
            mainStream = stream;

            using(var r = new BinaryReader(stream,Encoding.UTF8,true)) {
                var sig=r.ReadBytes(4);

                if(!sig.SequenceEqual(SIGNATURE_SDAT)) throw new InvalidDataException("SDAT signature is wrong");

                stream.Position = 14;
                var blockCount = r.ReadUInt16();
                var symbPos = r.ReadUInt32();
                var symbSize = r.ReadUInt32();
                var infoPos = r.ReadUInt32();
                var infoSize = r.ReadUInt32();
                var fatPos = r.ReadUInt32();
                var fatSize = r.ReadUInt32();

                files = parseFat(new SubStream(stream,fatPos,fatSize));
            }
        }

        private static List<FATRecord> parseFat(Stream stream) {
            using(var r=new BinaryReader(stream)) {
                var sig = r.ReadBytes(4);
                if(!sig.SequenceEqual(SIGNATURE_FAT)) throw new InvalidDataException("FAT signature is wrong");
                r.Skip(4);
                var numRecords = r.ReadInt32();

                List<FATRecord> files = new List<FATRecord>(numRecords);

                for(int recordIndex=0;recordIndex<numRecords;++recordIndex) {
                    var position = r.ReadUInt32();
                    var size = r.ReadUInt32();
                    files.Add(new FATRecord(position, size));
                    r.Skip(8);
                }

                return files;
            }
        }

        private class FATRecord {
            internal UInt32 size;
            internal UInt32 position;

            internal FATRecord(UInt32 size, UInt32 position) {
                this.size = size;
                this.position = position;
            }
        }
    }
}
