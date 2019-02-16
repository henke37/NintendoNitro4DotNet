using System;
using System.Collections.Generic;

namespace Nitro.Graphics {
	public class Icon {
		public Palette Palette;
		public Tile[] Tiles;

		public const int TilesX = 2;
		public const int TilesY = 2;

		public Icon(Tile[] tiles, Palette palette) {
			Tiles = tiles;
			Palette = palette;
		}
	}
}
