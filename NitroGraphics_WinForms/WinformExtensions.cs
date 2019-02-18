﻿using System;
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

		public static void DrawInBitmap(this Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false) {
			switch(bm.PixelFormat) {
				case PixelFormat.Format8bppIndexed:
					DrawTile8Bpp(tile, bm, left, top, flipX, flipY);
					return;
				case PixelFormat.Format4bppIndexed:
					DrawTile4Bpp(tile, bm, left, top, flipX, flipY);
					return;
				default:
					throw new NotSupportedException();
			}
		}

		private static void DrawTile8Bpp(Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false) { 
			Rectangle rect = new Rectangle {
				X=left, Y=top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd=bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			int byteCount = Tile.Width;
			byte[] pixelValues = new byte[byteCount];

			//Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y=0;y<Tile.Height;++y) {
				for(int x=0;x<Tile.Width;++x) {
					byte pixel = tile.TileData[Tile.MirrorX(x,flipX) + Tile.MirrorY(y,flipY) * Tile.Width];
					pixelValues[x+y*bmd.Stride] = pixel;
				}
				IntPtr dst = bmd.Scan0 + bmd.Stride * y;
				Marshal.Copy(pixelValues, 0, dst, byteCount);
			}

			bm.UnlockBits(bmd);
		}

		private static void DrawTile4Bpp(Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false) {
			Rectangle rect = new Rectangle {
				X = left,
				Y = top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd = bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
			int byteCount = Tile.Width/2;
			byte[] pixelValues = new byte[byteCount];

			//Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y = 0; y < Tile.Height; ++y) {
				for(int x = 0; x < Tile.Width; x+=2) {
					byte pixel1 = tile.TileData[Tile.MirrorX(x, flipX) + Tile.MirrorY(y, flipY) * Tile.Width];
					byte pixel2 = tile.TileData[Tile.MirrorX(x + 1, flipX) + Tile.MirrorY(y, flipY) * Tile.Width];
					int index = x / 2;
					pixelValues[index] = (byte)(pixel2 | (pixel1<<4));
				}
			
				IntPtr dst= bmd.Scan0 + bmd.Stride*y;
				Marshal.Copy(pixelValues, 0, dst, byteCount);
			}

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

		public static Bitmap ToBitmap(this Tilemap tilemap, GraphicsBank tileSet, Palette palette) {
			int widthTiles = tilemap.TilesX;
			int heightTiles = tilemap.TilesY;

			var bm = new Bitmap(widthTiles * Tile.Width, heightTiles * Tile.Height, tileSet.Format.AsPixelFormat());

			palette.Apply(bm);

			tilemap.DrawInBitmap(tileSet, bm);

			return bm;
		}

		public static void DrawInBitmap(this Tilemap tilemap, GraphicsBank tileSet, Bitmap bm) {
			for(int tileY=0;tileY<tilemap.TilesY;++tileY) {
				for(int tileX=0;tileX<tilemap.TilesX;++tileX) {
					Tilemap.TilemapEntry tileEntry = tilemap.TileMap[tileY, tileX];
					Tile tile = tileSet.Tiles[tileEntry.TileId];
					//BUG: Mirroring and palette offset not handled
					tile.DrawInBitmap(bm, tileX * Tile.Width, tileY * Tile.Height,tileEntry.XFlip,tileEntry.YFlip);
				}
			}
		}
	}
}
