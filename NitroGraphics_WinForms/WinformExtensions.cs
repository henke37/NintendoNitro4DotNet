using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Nitro.Graphics.WinForms {
	public static class WinformExtensions {
		public static Bitmap ToBitmap(this Tile tile, Nitro.Graphics.Palette nitroPal) {
			var bm = new Bitmap(Tile.Width, Tile.Height, PixelFormat.Format8bppIndexed);
			nitroPal.Apply(bm);
			tile.DrawInBitmap(bm);
			return bm;
		}

		public static void DrawInBitmap(this Tile tile, Bitmap bm, int left = 0, int top = 0) {
			switch(bm.PixelFormat) {
				case PixelFormat.Format8bppIndexed:
					DrawTile8Bpp(tile, bm, left, top);
					return;
				case PixelFormat.Format4bppIndexed:
					DrawTile4Bpp(tile, bm, left, top);
					return;
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

			//Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y=0;y<Tile.Height;++y) {
				for(int x=0;x<Tile.Width;++x) {
					byte pixel = tile.TileData[x + y * Tile.Width];
					pixelValues[x+y*bmd.Stride] = pixel;
				}
			}
			Marshal.Copy(pixelValues, 0, bmd.Scan0, byteCount);

			bm.UnlockBits(bmd);
		}

		private static void DrawTile4Bpp(Tile tile, Bitmap bm, int left = 0, int top = 0) {
			Rectangle rect = new Rectangle {
				X = left,
				Y = top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd = bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
			int byteCount = Math.Abs(bmd.Stride) * bm.Height;
			byte[] pixelValues = new byte[byteCount];

			//Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y = 0; y < Tile.Height; ++y) {
				for(int x = 0; x < Tile.Width; x+=2) {
					byte pixel1 = tile.TileData[x + y * Tile.Width];
					byte pixel2 = tile.TileData[x+1 + y * Tile.Width];
					int index = x / 2 + y * bmd.Stride;
					pixelValues[index] = (byte)(pixel1 | (pixel2<<4));
				}
			}
			Marshal.Copy(pixelValues, 0, bmd.Scan0, byteCount);

			bm.UnlockBits(bmd);
		}


		public static Color ToColor(this BGR555 clr) {
			return Color.FromArgb(clr.NormalizedR, clr.NormalizedG, clr.NormalizedB);
		}

		public static void Apply(this Palette pal, Bitmap bm) {
			var cpal = bm.Palette;
			pal.Apply(cpal);
			bm.Palette = cpal;
		}
		public static void Apply(this Palette pal, ColorPalette cpal) {
			for(int index = 0; index < cpal.Entries.Length; ++index) {
				cpal.Entries[index] = pal.Colors[index].ToColor();
			}
		}

		public static Bitmap ToBitmap(this Icon ico) {
			var bm = new Bitmap(Tile.Width * Icon.TilesX, Tile.Height * Icon.TilesY, PixelFormat.Format4bppIndexed);

			ico.Palette.Apply(bm);

			for(int tileY=0;tileY<Icon.TilesY;++tileY) {
				for(int tileX=0;tileX<Icon.TilesX;++tileX) {
					var tile = ico.Tiles[tileX + tileY * Icon.TilesX];
					tile.DrawInBitmap(bm, tileX * Tile.Width, tileY * Tile.Height);
				}
			}

			return bm;
		}

		public static PixelFormat AsPixelFormat(this TextureFormat fmt) {
			switch(fmt) {
				case TextureFormat.PLTT16: return PixelFormat.Format4bppIndexed;
				case TextureFormat.PLTT256: return PixelFormat.Format8bppIndexed;
				default: throw new NotSupportedException();
			}
		}
	}
}
