using Henke37.IOUtils;
using Henke37.Nitro.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AceAttorney.GK2 {
	public class MainArchive {

		private List<FileEntry> entries;
		private Stream mainStream;

		private long dataOffset;

		public MainArchive(Stream archiveStream) {
			mainStream = archiveStream;
			using(BinaryReader r = new BinaryReader(archiveStream, Encoding.Default, true)) {
				ReadTable(r);
				dataOffset = mainStream.Position;
			}
		}

		private void ReadTable(BinaryReader r) {
			entries = new List<FileEntry>();
			for(; ; ) {
				var entry = FileEntry.Load(r);
				if(entry.size == 0) break;
				entries.Add(entry);
			}
		}

		public Stream OpenFile(int fileId) {
			var entry = entries[fileId];
			Stream stream= new SubStream(mainStream, entry.offset, entry.size);

			if(entry.compressed) {
				stream = StockCompression.OpenCompressedStream(stream);
			}

			return stream;
		}

		public int FileCount => entries.Count;

		private class FileEntry {
			public uint offset;
			public uint size;
			public bool compressed;

			public FileEntry(uint offset, uint size, bool compressed) {
				this.offset = offset;
				this.size = size;
				this.compressed = compressed;
			}

			internal static FileEntry Load(BinaryReader r) {
				uint offset = r.ReadUInt32();
				uint size = r.ReadUInt32();
				bool compressed = (size & 0x80000000)==0x80000000;
				size &= 0x00FFFFFF;
				return new FileEntry(offset, size, compressed);
			}

			public override string ToString() {
				return String.Format("0x{0:X6} 0x{1:X6} {2}", offset, size, compressed);
			}
		}
	}
}
