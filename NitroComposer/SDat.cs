using Henke37.IOUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Nitro.Composer {
    public class SDat {

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

		public List<string> seqSymbols;
		public List<string> bankSymbols;
		public List<string> waveArchiveSymbols;
		public List<string> playerSymbols;
		public List<string> groupSymbols;
		public List<string> streamPlayerSymbols;
		public List<string> streamSymbols;

        public List<BankInfoRecord> bankInfo;
		public List<WaveArchiveInfoRecord> waveArchiveInfo;
        public List<PlayerInfoRecord> playerInfo;
        public List<GroupInfoRecord> groupInfo;
		public List<SequenceInfoRecord> sequenceInfo;
		public List<StreamInfoRecord> streamInfo;

		public SDat() {

        }

        private void Parse(Stream stream) {
            if(!stream.CanRead) throw new ArgumentException("Stream must be readable!", nameof(stream));
            if(!stream.CanSeek) throw new ArgumentException("Stream must be seekable!", nameof(stream));
            mainStream = stream;

            using(var r = new BinaryReader(stream, Encoding.UTF8, true)) {
                var sig = r.Read4C();
                if(sig!="SDAT") throw new InvalidDataException("SDAT signature is wrong");

                stream.Position = 14;
                var blockCount = r.ReadUInt16();
                var symbPos = r.ReadUInt32();
                var symbSize = r.ReadUInt32();
                var infoPos = r.ReadUInt32();
                var infoSize = r.ReadUInt32();
                var fatPos = r.ReadUInt32();
                var fatSize = r.ReadUInt32();

                files = parseFat(new SubStream(stream, fatPos, fatSize));
                parseInfo(new SubStream(stream, infoPos, infoSize));

                if(symbPos != 0) {
                    parseSymb(new SubStream(stream, symbPos, symbSize));
                }
            }
        }

        private void parseInfo(Stream stream) {
            using(var r = new BinaryReader(stream)) {
                var sig = r.Read4C();
                if(sig!="INFO") throw new InvalidDataException("INFO signature is wrong");
                var internalSize = r.ReadUInt32();
                if(internalSize != stream.Length) throw new InvalidDataException("INFO block size is wrong!");

                const int subsectionCount = 8;
                var subSectionPositions = r.ReadUInt32Array(subsectionCount);

                List<UInt32> ReadInfoRecordPtrTable(int subsectionIndex) {
                    stream.Position = subSectionPositions[subsectionIndex];
                    var recordCount = r.ReadUInt32();
                    return r.ReadUInt32Array((int)recordCount);
                }

                using(var subReader = new BinaryReader(new SubStream(stream, 0))) {
                    List<uint> recordPositions = ReadInfoRecordPtrTable(0);
                    sequenceInfo = new List<SequenceInfoRecord>(recordPositions.Count);
                    foreach(var position in recordPositions) {
                        if(position == 0) {
                            sequenceInfo.Add(null);
                            continue;
                        }
                        subReader.BaseStream.Position = position;
                        var record = SequenceInfoRecord.Read(subReader);
                        sequenceInfo.Add(record);
                    }
                }

                using(var subReader = new BinaryReader(new SubStream(stream, 0))) {
                    List<uint> recordPositions = ReadInfoRecordPtrTable(2);
                    bankInfo = new List<BankInfoRecord>(recordPositions.Count);
                    foreach(var position in recordPositions) {
                        if(position == 0) {
                            bankInfo.Add(null);
                            continue;
                        }
                        subReader.BaseStream.Position = position;
                        var record = BankInfoRecord.Read(subReader);
                        bankInfo.Add(record);
                    }
                }

                using(var subReader = new BinaryReader(new SubStream(stream, 0))) {
                    List<uint> recordPositions = ReadInfoRecordPtrTable(3);
                    waveArchiveInfo = new List<WaveArchiveInfoRecord>(recordPositions.Count);
                    foreach(var position in recordPositions) {
                        if(position == 0) {
                            waveArchiveInfo.Add(null);
                            continue;
                        }
                        subReader.BaseStream.Position = position;
                        WaveArchiveInfoRecord record = WaveArchiveInfoRecord.Read(subReader);
                        waveArchiveInfo.Add(record);
                    }
                }

                using(var subReader = new BinaryReader(new SubStream(stream, 0))) {
                    List<uint> recordPositions = ReadInfoRecordPtrTable(4);
                    playerInfo = new List<PlayerInfoRecord>(recordPositions.Count);
                    foreach(var position in recordPositions) {
                        if(position == 0) {
                            playerInfo.Add(null);
                            continue;
                        }
                        subReader.BaseStream.Position = position;
                        PlayerInfoRecord record = PlayerInfoRecord.Read(subReader);
                        playerInfo.Add(record);
                    }
                }

                using(var subReader = new BinaryReader(new SubStream(stream, 0))) {
                    List<uint> recordPositions = ReadInfoRecordPtrTable(5);
                    groupInfo = new List<GroupInfoRecord>(recordPositions.Count);
                    foreach(var position in recordPositions) {
                        if(position == 0) {
                            groupInfo.Add(null);
                            continue;
                        }
                        subReader.BaseStream.Position = position;
                        GroupInfoRecord record = GroupInfoRecord.Read(subReader);
                        groupInfo.Add(record);
                    }
                }

                using(var subStream = new SubStream(stream, 0)) {
                    List<uint> recordPositions = ReadInfoRecordPtrTable(7);
                    streamInfo = new List<StreamInfoRecord>(recordPositions.Count);
                    foreach(var position in recordPositions) {
                        if(position == 0) {
                            streamInfo.Add(null);
                            continue;
                        }
                        subStream.Position = position;
                        StreamInfoRecord record = StreamInfoRecord.Read(r);
                        streamInfo.Add(record);
                    }
                }
            }
        }

        private void parseSymb(SubStream symbStream) {
            using(var r = new BinaryReader(symbStream)) {
                var sig = r.Read4C();
                if(sig!="SYMB") throw new InvalidDataException("SYMB signature is wrong");
                var internalSize = r.ReadUInt32();
                //if(internalSize != stream.Length) throw new InvalidDataException("SYMB block size is wrong!");

                var stringReader = new BinaryReader(new SubStream(symbStream, 0));

                List<string> parseSymbSubRec(SubStream subStream) {
                    using(var r2 = new BinaryReader(subStream)) {
                        var nameCount = r2.ReadUInt32();
                        var names = new List<string>((int)nameCount);
                        for(UInt32 nameIndex = 0; nameIndex < nameCount; ++nameIndex) {
                            subStream.Position = 4 + 4 * nameIndex;
                            var stringPos = r2.ReadUInt32();

                            if(stringPos == 0) {
                                names.Add(null);
                                continue;
                            }

                            stringReader.Seek((int)stringPos);
                            names.Add(stringReader.ReadNullTerminatedUTF8String());
                        }
                        return names;
                    }

                }

				var seqSymbPtr = r.ReadUInt32();
				r.Skip(4);
				var bankSymbPtr = r.ReadUInt32();
				var waveArchSymbPtr = r.ReadUInt32();
				var playerSymbPtr = r.ReadUInt32();
				var groupSymbPtr = r.ReadUInt32();
				var streamPlayerSymbPtr = r.ReadUInt32();
				var streamSymbPtr = r.ReadUInt32();

				if(seqSymbPtr != 0) seqSymbols = parseSymbSubRec(new SubStream(symbStream, seqSymbPtr));
				if(bankSymbPtr != 0) bankSymbols = parseSymbSubRec(new SubStream(symbStream, bankSymbPtr));
				if(waveArchSymbPtr != 0) waveArchiveSymbols = parseSymbSubRec(new SubStream(symbStream, waveArchSymbPtr));
				if(playerSymbPtr != 0) playerSymbols = parseSymbSubRec(new SubStream(symbStream, playerSymbPtr));
				if(groupSymbPtr != 0) groupSymbols = parseSymbSubRec(new SubStream(symbStream, groupSymbPtr));
				if(streamPlayerSymbPtr != 0) streamPlayerSymbols = parseSymbSubRec(new SubStream(symbStream, streamPlayerSymbPtr));
				if(streamSymbPtr != 0) streamSymbols = parseSymbSubRec(new SubStream(symbStream, streamSymbPtr));
            }
        }

        private static List<FATRecord> parseFat(Stream stream) {
            using(var r = new BinaryReader(stream)) {
                var sig = r.Read4C();
                if(sig!="FAT ") throw new InvalidDataException("FAT signature is wrong");
                r.Skip(4);
                var numRecords = r.ReadInt32();

                List<FATRecord> files = new List<FATRecord>(numRecords);

                for(int recordIndex = 0; recordIndex < numRecords; ++recordIndex) {
                    var size = r.ReadUInt32();
                    var position = r.ReadUInt32();
                    files.Add(new FATRecord(position, size));
                    r.Skip(8);
                }

                return files;
            }
        }

        internal Stream OpenSubFile(int fatId) {
            var record = files[fatId];
            return new SubStream(mainStream, record.position, record.size);
        }

        public STRM OpenStream(string name) {
            int streamIndex = ResolveStreamName(name);
            if(streamIndex == -1) throw new FileNotFoundException();
            return OpenStream(streamIndex);
        }

        public STRM OpenStream(int streamIndex) {
            var infoRecord = streamInfo[streamIndex];
            return new STRM(OpenSubFile(infoRecord.fatId));
        }

        public SSEQ OpenSequence(string name) {
            int sequenceIndex = ResolveSeqName(name);
            if(sequenceIndex == -1) throw new FileNotFoundException();
            return OpenSequence(sequenceIndex);
        }

        public SSEQ OpenSequence(int sequenceIndex) {
            var infoRecord = sequenceInfo[sequenceIndex];
            return new SSEQ(OpenSubFile(infoRecord.fatId));
        }

        public SBNK OpenBank(string name) {
            int bankIndex = ResolveBankName(name);
            if(bankIndex == -1) throw new FileNotFoundException();
            return OpenBank(bankIndex);
        }

        public SBNK OpenBank(int bankIndex) {
            var infoRecord = bankInfo[bankIndex];
            return new SBNK(OpenSubFile(infoRecord.fatId));
        }

        public SWAR OpenWaveArchive(string name) {
            int waveArchiveIndex = ResolveWaveArchiveName(name);
            if(waveArchiveIndex == -1) throw new FileNotFoundException();
            return OpenWaveArchive(waveArchiveIndex);
        }

        public SWAR OpenWaveArchive(int bankIndex) {
            var infoRecord = waveArchiveInfo[bankIndex];
            return new SWAR(OpenSubFile(infoRecord.fatId));
        }

		public int ResolveStreamName(string name) {
			if(streamSymbols is null) throw new NoSymbolsException("No sequence symbols");
			return streamSymbols.IndexOf(name);
		}

		public int ResolveSeqName(string name) {
			if(seqSymbols is null) throw new NoSymbolsException("No sequence symbols");
			return seqSymbols.IndexOf(name);
		}

		public int ResolveBankName(string name) {
			if(bankSymbols is null) throw new NoSymbolsException("No bank symbols");
			return bankSymbols.IndexOf(name);
		}

		public int ResolveWaveArchiveName(string name) {
			if(waveArchiveSymbols is null) throw new NoSymbolsException("No wave archive symbols");
			return waveArchiveSymbols.IndexOf(name);
		}

		private class FATRecord {
            internal UInt32 size;
            internal UInt32 position;

            internal FATRecord(UInt32 size, UInt32 position) {
                this.size = size;
                this.position = position;
            }
        }

        public class SequenceInfoRecord {
            public ushort fatId;
            public ushort bankId;
            public byte vol;
            public byte channelPriority;
            public byte playerPriority;
            public byte player;

            public static SequenceInfoRecord Read(BinaryReader r) {
                var record = new SequenceInfoRecord();
                record.fatId = r.ReadUInt16();
                r.Skip(2);
                record.bankId = r.ReadUInt16();
                record.vol = r.ReadByte();
                record.channelPriority = r.ReadByte();
                record.playerPriority = r.ReadByte();
                record.player = r.ReadByte();

                return record;
            }
        }

        public class StreamInfoRecord {
            public ushort fatId;
            public byte vol;
            public byte priority;
            public byte player;
            public bool forceStereo;

            public static StreamInfoRecord Read(BinaryReader r) {
                var record = new StreamInfoRecord();
                record.fatId = r.ReadUInt16();
                r.Skip(2);
                record.vol = r.ReadByte();
                record.priority = r.ReadByte();
                record.player = r.ReadByte();
                record.forceStereo = r.ReadBoolean();

                return record;
            }
        }

        public class BankInfoRecord {
            public UInt16 fatId;
            public List<short> swars;

            internal static BankInfoRecord Read(BinaryReader r) {
                var record = new BankInfoRecord();
                record.fatId = r.ReadUInt16();
				r.Skip(2);//presumably padding
                record.swars = r.ReadInt16Array(4);
                return record;
            }
        }

        public class WaveArchiveInfoRecord {
            public UInt16 fatId;
            internal static WaveArchiveInfoRecord Read(BinaryReader r) {
                var record = new WaveArchiveInfoRecord();
                record.fatId = r.ReadUInt16();
                return record;
            }
        }

        public class PlayerInfoRecord {
            public byte maxSequences;
            public ushort channelMask;
            public uint heapSize;

            internal static PlayerInfoRecord Read(BinaryReader r) {
                var record = new PlayerInfoRecord();
                record.maxSequences = r.ReadByte();
                r.Skip(1);
                record.channelMask = r.ReadUInt16();
                record.heapSize = r.ReadUInt32();
                return record;
            }
        }

        public class GroupInfoRecord {
            private List<ElementRecord> elements;

            internal static GroupInfoRecord Read(BinaryReader r) {
                var record = new GroupInfoRecord();
                var elementCount = r.ReadUInt32();
                record.elements = new List<ElementRecord>((int)elementCount);
                for(UInt32 elementIndex=0;elementIndex<elementCount;++elementIndex) {
                    var element = new ElementRecord();
                    element.Read(r);
                    record.elements.Add(element);
                }
                return record;
            }

            public class ElementRecord {
                public byte itemType;
                public byte flags;
                public uint itemId;

                public ElementRecord() {
                }

                internal void Read(BinaryReader r) {
                    itemType = r.ReadByte();
                    flags = r.ReadByte();
                    r.Skip(2);
                    itemId = r.ReadUInt32();
                }
            }
        }

		[Serializable]
		public class NoSymbolsException : InvalidOperationException {
			public NoSymbolsException(string message) : base(message) {
			}

			protected NoSymbolsException(SerializationInfo info, StreamingContext context) : base(info, context) {
			}
		}
	}
}
