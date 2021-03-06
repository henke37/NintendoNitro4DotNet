﻿using Henke37.Nitro;
using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Graphics {
	public class NCGR : GraphicsBank {
		private ushort GridX;
		private ushort GridY;

		public NCGR(Stream stream) {
			Load(stream);
		}

		private void Load(Stream stream) {
			var sections = new SectionedFile(stream);
			if(sections.MainId != "RGCN") throw new InvalidDataException();
			ParseRAHC(sections.Open("RAHC"));
		}

		private void ParseRAHC(Stream stream) {
			using(var r=new BinaryReader(stream)) {
				TilesX = r.ReadUInt16();
				TilesY = r.ReadUInt16();

				Format = (TextureFormat)r.ReadUInt32();//format

				GridX = r.ReadUInt16();
				GridY = r.ReadUInt16();

				Mapping = (MappingMode)r.ReadUInt16();
				var charMode=r.ReadUInt16();

				int dataSize = r.ReadInt32();
				var dataOffset = r.ReadUInt32();

				stream.Position = dataOffset;

				int tileCount = BytesToPixels(dataSize) / (Tile.Width * Tile.Height);

				switch(Mapping) {
					case MappingMode.Char_2D:
						ParseTiled(r, tileCount);
						break;
					default:
						ParseScanned(r, tileCount);
						break;
				}
			}
		}
	}
}
