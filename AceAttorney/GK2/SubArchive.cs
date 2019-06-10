using Henke37.IOUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AceAttorney.GK2 {
	public class SubArchive {

		private List<long> offsets;

		private Stream mainStream;

		public SubArchive(Stream stream) {
			mainStream = stream;

			using(var r = new BinaryReader(stream,Encoding.Default,true)) {
				offsets = new List<long>();

				long firstFile = r.ReadUInt32();
				offsets.Add(firstFile);
				while(stream.Position < firstFile) {
					var offset = r.ReadUInt32();
					offsets.Add(offset);
				}
			}
		}

		public Stream OpenFile(int fileId) {
			if(fileId < 0) throw new ArgumentOutOfRangeException("Fileid can't be negative", nameof(fileId));

			if(fileId+1==offsets.Count) {
				return new SubStream(mainStream, offsets[fileId]);
			} else {
				var len = offsets[fileId + 1] - offsets[fileId];
				return new SubStream(mainStream, offsets[fileId], len);
			}
		}
	}
}
