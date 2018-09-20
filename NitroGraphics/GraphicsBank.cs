using System;
using System.Collections.Generic;
using System.IO;

namespace Nitro.Graphics {
	public class GraphicsBank {

		public ushort TilesX;
		public ushort TilesY;

		public TextureFormat Format;

		public Tile[] Tiles;
		public byte[] Pixels;

		protected void ParseTiled(BinaryReader reader, int tileCount) {
			Tiles = new Tile[tileCount];
			for(int tileIndex=0;tileIndex<tileCount;++tileIndex) {
				Tiles[tileIndex]=reader.ReadTile(Format);
			}
		}

		protected void ParseScanned(BinaryReader reader, int pixelCount) {
			Pixels = new byte[pixelCount];
			switch(Format) {
				case TextureFormat.PLTT4: {
					for(int i=0;i<pixelCount;) {
						byte b = reader.ReadByte();
						Pixels[i++] = (byte)(b & 3);
						b >>= 2;
						Pixels[i++] = (byte)(b & 3);
						b >>= 2;
						Pixels[i++] = (byte)(b & 3);
						b >>= 2;
						Pixels[i++] = (byte)(b & 3);
					}
				} break;
				case TextureFormat.PLTT16: {
					for(int i = 0; i < pixelCount;) {
						byte b = reader.ReadByte();
						Pixels[i++] = (byte)(b & 0xF);
						Pixels[i++] = (byte)(b >> 4);
					}
				} break;
				case TextureFormat.PLTT256: {
					for(int i = 0; i < pixelCount;) {
						Pixels[i++] = reader.ReadByte();
					}
				} break;
				default:
					throw new NotSupportedException();
			}
		}
	}
}