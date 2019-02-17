using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HenkesUtils;

namespace Nitro {
	public class Banner {

		public string[] Titles;
		private const int titleLength = 0x80;

		internal SubStream IconPixels;
		internal SubStream IconPalette;

		public Banner(Stream stream) {
			Load(stream);
		}

		private void Load(Stream stream) {
			using(var r = new BinaryReader(stream, Encoding.Unicode)) {
				UInt16 version = r.ReadUInt16();
				List<UInt16> checksums = r.ReadUInt16Array(4);
				r.Skip(0x16);

				IconPixels=r.ReadSubStream(0x200);
				IconPalette = r.ReadSubStream(0x20);

				int titleCount = version >= 3 ? 8 : (version >= 2 ? 7 : 6);

				Titles = new string[titleCount];
				for(int titleIndex = 0; titleIndex < titleCount; ++titleIndex) {
					Titles[titleIndex] = new string(r.ReadChars(titleLength)).TrimEnd('\0');
				}
			}
		}

		public enum TitleId {
			Japanese = 0,
			English,
			French,
			German,
			Italian,
			Spanish,
			Chinese,
			Korean
		}
	}
}