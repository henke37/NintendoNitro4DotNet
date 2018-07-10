using System;
using System.Collections.Generic;
using System.IO;
using HenkesUtils;

namespace Nitro {
	public class Banner {

		public string[] Titles;

		private const int titleCount = 8;
		private const int titleLength = 0x100;

		public Banner(Stream stream) {
			Load(stream);
		}

		private void Load(Stream stream) {
			using(var r=new BinaryReader(stream)) {
				UInt16 version = r.ReadUInt16();
				List<UInt16> checksums = r.ReadUInt16Array(4);
				r.Skip(0x16);

				r.Skip(0x200 + 0x20);//Icon

				Titles = new string[titleCount];
				for(int titleIndex=0;titleIndex<titleCount;++titleIndex) {
					Titles[titleIndex] = new string(r.ReadChars(titleLength));
				}
			}
		}
	}
}