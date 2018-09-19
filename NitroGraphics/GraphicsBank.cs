using System.Collections.Generic;
using System.IO;

namespace Nitro.Graphics {
	public class GraphicsBank {

		public ushort TilesX;
		public ushort TilesY;

		public TextureFormat Format;

		public List<Tile> Tiles;

		public void ParseTiled(BinaryReader reader, int tileCount) {
			Tiles = new List<Tile>(tileCount);
			for(int tileIndex=0;tileIndex<tileCount;++tileIndex) {
				Tiles.Add(reader.ReadTile(Format));
			}
		}
	}
}