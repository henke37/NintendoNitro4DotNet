using System;
using System.Collections.Generic;
using System.IO;

namespace Nitro.Graphics {
	public class NCLR {
		public Palette Palette;

		public NCLR(int palSize) {
			Palette = new Palette(new List<BGR555>(palSize));
		}

		public NCLR(List<BGR555> pal) {
			Palette = new Palette(pal);
		}

		public NCLR(Stream stream) {
			Load(stream);
		}

		public void Load(Stream stream) {
			var sections = new SectionedFile(stream);
			if(sections.MainId!="RLCN") throw new InvalidDataException();
			var section = sections.Open("TTLP");
			using(var r= new BinaryReader(section)) {
				var format=r.ReadUInt32();//format
				var extPal=r.ReadUInt32();//extpal used
				var byteSize=r.ReadUInt32();
				var pointer=r.ReadUInt32();

				int paletteLength = (int)(byteSize / BGR555.DataSize);

				stream.Position = pointer;

				var pal = new List<BGR555>(paletteLength);
				for(int colorIndex=0;colorIndex<paletteLength;++colorIndex) {
					pal.Add(r.ReadBGR555());
				}
				Palette = new Palette(pal);
			}
		}
	}
}
