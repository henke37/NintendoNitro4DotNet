using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Graphics {
	public class GraphicsBank {

		public ushort TilesX;
		public ushort TilesY;

		public TextureFormat Format;
		protected MappingMode Mapping;

		public Tile[] Tiles;
		public byte[] Pixels;

		public bool CanBeDrawnStandalone { get => TilesX != 0xFFFF && TilesY != 0xFFFF; }

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

		protected enum MappingMode {
			Char_2D = 0,
			Char_1D_32K = 1,
			Char_1D_64K = 2,
			Char_1D_128K = 3,
			Char_1D_126K = 4
		}

		protected int BytesToPixels(int bytes) {
			switch(Format) {
				case TextureFormat.A3I5:
				case TextureFormat.A5I3:
				case TextureFormat.Direct:
					return bytes / 2;
				case TextureFormat.PLTT256:
					return bytes;
				case TextureFormat.PLTT16:
					return bytes * 2;
				case TextureFormat.PLTT4:
					return bytes * 4;
				case TextureFormat.COMP4x4:
					throw new NotImplementedException();
				case TextureFormat.None:
				default:
					throw new InvalidOperationException();
			}
		}
	}
}