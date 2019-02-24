using System;
using System.IO;

namespace Nitro.Graphics {
	public class OAMEntry {
		public uint PaletteIndex;
		public uint TileIndex;

		public uint TilesX;
		public uint TilesY;

		public uint Width { get => TilesX * Tile.Width; }
		public uint Height { get => TilesY * Tile.Height; }

		public int X;
		public int Y;
		public ObjectDisplayMode Mode;
		private int colordepth;
		public bool XFlip;
		public bool YFlip;
		private int Priority;

		public void Load(BinaryReader reader, int tileIndexShift, uint tileIndexNudge) {
			Y = reader.ReadByte();
			var atts0 = reader.ReadByte();
			Mode = (ObjectDisplayMode)(atts0 & 3);

			colordepth = (atts0 >> 5) & 0x1;

			var shape = atts0 >> 6;

			var atts1 = reader.ReadUInt16();
			X = atts1 & 0x1FF;
			if(X >= 0x100) X -= 0x200;

			if(Mode == ObjectDisplayMode.Normal || Mode == ObjectDisplayMode.Hidden) {
				XFlip = (atts1 & 0x1000) != 0;
				YFlip = (atts1 & 0x2000) != 0;
			}

			uint objSize = (uint)(atts1 >> 14);

			var atts2 = reader.ReadUInt16();

			TileIndex = (uint)(atts2 & 0x3FF);
			TileIndex <<= tileIndexShift;
			TileIndex += tileIndexNudge;

			Priority = (atts1 >> 10) & 0x3;

			setSize(objSize, shape);
		}

		private void setSize(uint objSize, int shape) {
			switch(shape) {
				case 0:
					TilesX = TilesY = (uint)1<<((int)objSize);
					break;
				case 1:
					TilesX = WHFast[objSize];
					TilesY = WHSlow[objSize];
					break;
				case 2:
					TilesX = WHSlow[objSize];
					TilesY = WHFast[objSize];
					break;
				default:
					throw new Exception();
			}
		}

		private static readonly uint[] WHFast = new uint[] { 2, 4, 4, 8 };
		private static readonly uint[] WHSlow = new uint[] { 1, 1, 2, 4 };

		public enum ObjectDisplayMode {
			Hidden = 2,
			Normal = 0,
			Affine = 1,
			AffineExtraRegion = 3
		}

		public override string ToString() {
			return string.Format("{0} {1}x{2} ({3},{4}) {5}{6} P{7} PL{8}",
				TileIndex,
				Width, Height,
				X, Y,
				XFlip ? "~X" : "",
				YFlip ? "~Y" : "",
				Priority,
				PaletteIndex
			);
		}
	}
}
