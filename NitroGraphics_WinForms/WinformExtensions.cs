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

		public static void DrawInBitmap(this Tile tile, Bitmap bm, int left = 0, int top = 0) {
			switch(bm.PixelFormat) {
				case PixelFormat.Format8bppIndexed:
					DrawTile8Bpp(tile, bm, left, top);
					return;
				case PixelFormat.Format4bppIndexed:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
			}
		}

		private static void DrawTile8Bpp(Tile tile, Bitmap bm, int left = 0, int top = 0) { 
			Rectangle rect = new Rectangle {
				X=left, Y=top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd=bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			int byteCount = Math.Abs(bmd.Stride) * bm.Height;
			byte[] pixelValues = new byte[byteCount];

			System.Runtime.InteropServices.Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y=0;y<Tile.Height;++y) {
				for(int x=0;x<Tile.Width;++x) {
					byte pixel = tile.TileData[x + y * Tile.Width];
					pixelValues[x+left+(y+top)*bmd.Stride] = pixel;
				}
			}
			System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmd.Scan0, byteCount);

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
