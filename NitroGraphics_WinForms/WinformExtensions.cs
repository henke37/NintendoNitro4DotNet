using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Nitro.Graphics.WinForms {
	public static class WinformExtensions {
		public static Bitmap ToBitmap(this Tile tile, Nitro.Graphics.Palette nitroPal) {
			var bm = new Bitmap(Tile.Width, Tile.Height, PixelFormat.Format8bppIndexed);
			nitroPal.Apply(bm);
			tile.DrawInBitmap(bm);
			return bm;
		}

		public static void DrawInBitmap(this Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false, int paletteOffset = 0) {
			switch(bm.PixelFormat) {
				case PixelFormat.Format8bppIndexed:
					DrawTile8Bpp(tile, bm, left, top, flipX, flipY, paletteOffset);
					return;
				case PixelFormat.Format4bppIndexed:
					if(paletteOffset != 0) throw new ArgumentException("4bpp bitmaps do not support palette offsets",nameof(paletteOffset));
					DrawTile4Bpp(tile, bm, left, top, flipX, flipY);
					return;
				default:
					throw new ArgumentException("Unsupported bitmap format",nameof(bm));
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		private static void DrawTile8Bpp(Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false, int paletteOffset = 0) { 
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
					byte pixel = (byte)( tile.TileData[Tile.MirrorX(x,flipX) + Tile.MirrorY(y,flipY) * Tile.Width] + paletteOffset );
					pixelValues[x] = pixel;
				}
				IntPtr dst = bmd.Scan0 + bmd.Stride * y;
				Marshal.Copy(pixelValues, 0, dst, byteCount);
			}

			bm.UnlockBits(bmd);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
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
				default: throw new ArgumentException("Format not supported",nameof(fmt));
			}
		}

		public static Bitmap ToBitmap(this Tilemap tilemap, GraphicsBank tileSet, Palette palette) {
			int widthTiles = tilemap.TilesX;
			int heightTiles = tilemap.TilesY;

			var bm = new Bitmap(widthTiles * Tile.Width, heightTiles * Tile.Height, PixelFormat.Format8bppIndexed);

			palette.Apply(bm);

			tilemap.DrawInBitmap(tileSet, bm);

			return bm;
		}

		public static void DrawInBitmap(this Tilemap tilemap, GraphicsBank tileSet, Bitmap bm) {
			for(int tileY=0;tileY<tilemap.TilesY;++tileY) {
				for(int tileX=0;tileX<tilemap.TilesX;++tileX) {
					Tilemap.TilemapEntry tileEntry = tilemap.TileMap[tileY, tileX];
					Tile tile = tileSet.Tiles[tileEntry.TileId];
					tile.DrawInBitmap(bm, tileX * Tile.Width, tileY * Tile.Height,tileEntry.XFlip,tileEntry.YFlip,tileEntry.Palette*16);
				}
			}
		}

		public static Bitmap ToBitmap(this GraphicsBank tiles, Palette palette) {
			int widthTiles = tiles.TilesX;
			int heightTiles = tiles.TilesY;

			var bm = new Bitmap(widthTiles * Tile.Width, heightTiles * Tile.Height, tiles.Format.AsPixelFormat());

			palette.Apply(bm);

			tiles.DrawInBitmap(bm);

			return bm;
		}

		public static void DrawInBitmap(this GraphicsBank tiles, Bitmap bm) {
			if(!tiles.CanBeDrawnStandalone) throw new NotSupportedException("Graphicsbank can't be drawn independently");

			int widthTiles = tiles.TilesX;
			int heightTiles = tiles.TilesY;

			for(int tileY = 0; tileY < heightTiles; ++tileY) {
				for(int tileX = 0; tileX < widthTiles; ++tileX) {
					var tile = tiles.Tiles[tileY * tiles.TilesX + tileX];
					tile.DrawInBitmap(bm, tileX * Tile.Width, tileY * Tile.Height);
				}
			}
		}
	}
}
