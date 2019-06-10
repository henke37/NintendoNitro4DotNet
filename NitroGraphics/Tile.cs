using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Graphics {
	public class Tile {

		public byte[] TileData;

		public const int Width = 8;
		public const int Height = 8;

		public Tile() {
			TileData = new byte[Width * Height];
		}

		public Tile(byte[] tileData) {
			TileData = tileData;
		}

		public Tile(BinaryReader reader, TextureFormat format) {
			TileData = new byte[Width * Height];
			switch(format) {
				case TextureFormat.PLTT16: {
					int index = 0;
					for(int y = 0; y < Height; ++y) {
						for(int x = 0; x < Width;x+=2) {
							var b = reader.ReadByte();
							TileData[index++] = (byte)(b & 0x0F);
							TileData[index++] = (byte)(b>>4 & 0x0F);
						}
					}
				} break;
				case TextureFormat.PLTT256: {
					int index = 0;
					for(int y = 0; y < Height; ++y) {
						for(int x = 0; x < Width; ++x) {
							TileData[index++] = reader.ReadByte();
						}
					}
				}
				break;

				default:
					throw new NotSupportedException();
			}
		}

		public static int MirrorX(int x, bool flipX) { if(flipX) return Tile.Width - (x + 1); return x; }
		public static int MirrorY(int y, bool flipY) { if(flipY) return Tile.Height - (y + 1); return y; }
	}
}
