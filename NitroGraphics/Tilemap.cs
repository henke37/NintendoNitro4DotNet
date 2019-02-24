using System;
using System.IO;

namespace Nitro.Graphics {
	public class Tilemap {

		public TilemapEntry[,] TileMap;

		public int TilesX { get => TileMap.GetLength(1); }
		public int TilesY { get => TileMap.GetLength(0); }

		protected TileMapFormat Format;

		protected enum TileMapFormat {
			Text,
			Affine,
			AffineExt
		}

		protected void LoadMap(BinaryReader reader) {
			switch(Format) {
				case TileMapFormat.Text:
					LoadTextMap(reader);
					break;
				case TileMapFormat.Affine:
					LoadAffineMap(reader);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		protected void LoadTextMap(BinaryReader reader) {
			for(var y = 0; y < TileMap.GetLength(0); ++y) {
				for(var x = 0; x < TileMap.GetLength(0); ++x) {
					var tile = new TilemapEntry();
					var v = reader.ReadUInt16();
					tile.TileId = v & 0x3FF;
					tile.XFlip = (v & 0x400)!=0;
					tile.YFlip = (v & 0x800)!=0;
					tile.Palette = (uint)(v >> 12);
					TileMap[y, x] = tile;
				}
			}
		}


		protected void LoadAffineMap(BinaryReader reader) {
			for(var y = 0; y < TileMap.GetLength(0); ++y) {
				for(var x = 0; x < TileMap.GetLength(0); ++x) {
					var tile = new TilemapEntry();
					tile.TileId = reader.ReadByte();
					TileMap[y, x] = tile;
				}
			}
		}

		public struct TilemapEntry {
			public int TileId;
			public bool XFlip;
			public bool YFlip;
			public uint Palette;
		}
	}
}