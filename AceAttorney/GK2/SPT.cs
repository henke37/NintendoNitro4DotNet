using Henke37.IOUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AceAttorney.GK2 {
	public class SPT {

		private List<SectionEntry> sections;

		public int SectionCount { get => sections.Count; }

		private Stream mainStream;

		private byte[] scramblingKey;
		private static readonly byte[] noScramblingKey = new byte[] { 0x00, 0x00 };

		public SPT(Stream stream) {

			using(var r = new BinaryReader(stream, Encoding.Default, true)) {
				if(r.Read4C() != " TPS") throw new InvalidDataException("Bad 4C");

				int version = r.ReadUInt16();
				int sectionCount = r.ReadUInt16();
				int sizeThing = r.ReadUInt16();
				scramblingKey = r.ReadBytes(2);

				//Fanpatch hack
				int offsetScale = scramblingKey.Equals(noScramblingKey) ? 1 : 0;

				sections = new List<SectionEntry>(sectionCount);

				for(int sectionIndex = 0; sectionIndex < sectionCount; ++sectionIndex) {
					var entry = new SectionEntry();
					entry.offset = r.ReadUInt16() << offsetScale;
					entry.size = r.ReadUInt16();
					entry.flags = r.ReadUInt32();
					sections.Add(entry);
				}
			}

			this.mainStream = stream;
		}

		public string OpenSection(int sectionId) {
			var entry = sections[sectionId];
			Stream sectionStream = new SubStream(mainStream, entry.offset, entry.size*2);
			if(!scramblingKey.Equals(noScramblingKey)) {
				sectionStream = new XorStream(sectionStream, scramblingKey);
			}

			var sb = new StringBuilder();

			var dec = Encoding.Unicode.GetDecoder();
			var txtBuff = new List<byte>();

			void addTxt() {
				byte[] arrB = txtBuff.ToArray();
				char[] arrC = new char[arrB.Length / 2];
				dec.GetChars(arrB, 0, arrB.Length, arrC, 0);
				sb.Append(arrC);
				txtBuff.Clear();
			}

			using(var r = new BinaryReader(sectionStream, Encoding.Unicode)) {
				for(int pos = 0; pos < entry.size; ++pos) {
					var x = r.ReadUInt16();
					switch(x >> 8) {
						case 0xE0:
						case 0xE1:
						case 0xE2:
						case 0xE3:
						case 0xE4:
						case 0xE5:
						case 0xE6:
						case 0xE7:
						case 0xE8:
						case 0xE9:
						case 0xEA:
						case 0xEB:
						case 0xEC:
						case 0xED:
						case 0xEE:
						case 0xEF:
						case 0xF0:
						case 0xF1:
						case 0xF2:
						case 0xF3:
						case 0xF4:
						case 0xF5:
						case 0xF6:
						case 0xF7:
						case 0xF8:
							addTxt();
							pos+=readControlCode(x, r);
							continue;
					}
					txtBuff.Add((byte)(x & 0xFF));
					txtBuff.Add((byte)(x >> 8));
				}
				if(txtBuff.Count > 0) addTxt();
			}

			return sb.ToString();
		}

		private int readControlCode(int cmd, BinaryReader reader) {
			switch(cmd) {
				default:
					int argC = 1;//HACK: should've extracted the ARGC table from the game when I could
					reader.Skip(argC * 2);
					return argC;
			}
		}

		private class SectionEntry {
			public int offset;
			public int size;
			public UInt32 flags;
		}
	}
}
