using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Nitro.Graphics.WinForms {
	public static class WinformExtensions {
		public static Bitmap ToBitmap(this Tile tile, Nitro.Graphics.Palette nitroPal) {
			var bm = new Bitmap(Tile.Width, Tile.Height, PixelFormat.Format8bppIndexed);
			nitroPal.ApplyToBitmap(bm);
			tile.DrawInBitmap(bm);
			return bm;
		}

		public static void DrawInBitmap(this Tile tile, Bitmap bm, int left=0, int top=0) {
			Rectangle rect = new Rectangle {
				X=left, Y=top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd=bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Indexed);
			int byteCount = Math.Abs(bmd.Stride) * bm.Height;
			byte[] pixelValues = new byte[byteCount];

			for(int y=0;y<Tile.Height;++y) {
				for(int x=0;x<Tile.Width;++x) {
					pixelValues[x+y*bmd.Stride] = tile.TileData[x + y * Tile.Width];
				}
			}
			System.Runtime.InteropServices.Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			bm.UnlockBits(bmd);
		}

		public static Color ToColor(this BGR555 clr) {
			return Color.FromArgb(clr.NormalizedR, clr.NormalizedG, clr.NormalizedB);
		}

		public static void ApplyToBitmap(this Palette pal, Bitmap bm) {
			for(int index = 0; index < bm.Palette.Entries.Length; ++index) {
				bm.Palette.Entries[index] = pal.Colors[index].ToColor();
			}
		}
	}
}
