using Nitro;
using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Graphics {
	public static class Extensions {
		public static BGR555 ReadBGR555(this BinaryReader reader) {
			var u = reader.ReadUInt16();
			return (BGR555)(u);
		}

		public static Tile ReadTile(this BinaryReader reader, TextureFormat format) {
			return new Tile(reader, format);
		}

		public static OAMEntry ReadOAMEntry(this BinaryReader reader, int tileIndexShift, uint tileIndexNudge) {
			var o = new OAMEntry();
			o.Load(reader, tileIndexShift, tileIndexNudge);
			return o;
		}

		public static Icon Icon(this Banner banner) {
			const int tileCount = Graphics.Icon.TilesX * Graphics.Icon.TilesY;
			Tile[] tiles = new Tile[tileCount];
			using(var r = new BinaryReader(banner.IconPixels)) {
				for(int tileIndex = 0; tileIndex < tileCount; ++tileIndex) {
					tiles[tileIndex] = new Tile(r, TextureFormat.PLTT16);
				}
			}

			BGR555[] palette=new BGR555[16];
			using(var r=new BinaryReader(banner.IconPalette)) {
				for(int colorIndex=0;colorIndex<16;++colorIndex) {
					palette[colorIndex] = r.ReadBGR555();
				}
			}
			return new Icon(tiles, new Palette(palette));
		}
	}
}
