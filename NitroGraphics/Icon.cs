using System;
using System.Collections.Generic;

namespace Henke37.Nitro.Graphics {
	public class Icon {
		public Palette Palette;
		public Tile[] Tiles;

		public const int TilesX = 4;
		public const int TilesY = 4;

		public Icon(Tile[] tiles, Palette palette) {
			Tiles = tiles;
			Palette = palette;
		}
	}
}
