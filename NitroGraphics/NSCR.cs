using Henke37.Nitro;
using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Graphics {
	public class NSCR : Tilemap {

		public NSCR() { }

		public NSCR(Stream stream) {
			var sections = new SectionedFile(stream);
			if(sections.MainId != "RCSN") throw new InvalidDataException("Bad signature");

			using(var r=new BinaryReader(sections.Open("NRCS"))) {
				int WidthPixels = r.ReadUInt16();
				int HeightPixel = r.ReadUInt16();

				if((WidthPixels % Tile.Width) != 0) throw new InvalidDataException("Width not an even multipiel");
				if((WidthPixels % Tile.Height) != 0) throw new InvalidDataException("Height not an even multipiel");

				var colorMode = (ColorMode)r.ReadUInt16();
				Format = (TileMapFormat)r.ReadUInt16();

				var WidthTiles = WidthPixels / Tile.Width;
				var HeightTiles = HeightPixel / Tile.Height;

				TileMap = new TilemapEntry[HeightTiles, WidthTiles];

				var dataSize = r.ReadUInt32();
				LoadMap(r);
			}
		}

		enum ColorMode {
			CM_16x16,
			CM_256x1,
			CM_256x16
		}
	}
}
