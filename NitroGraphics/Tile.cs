using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Graphics {
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
	}
}
