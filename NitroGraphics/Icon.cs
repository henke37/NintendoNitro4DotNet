using System;
using System.Collections.Generic;

namespace Nitro.Graphics {
	public class Icon {
		public Palette Palette;
		public Tile[] Tiles;

		public Icon(Tile[] tiles, Palette palette) {
			Tiles = tiles;
			Palette = palette;
		}
	}
}
