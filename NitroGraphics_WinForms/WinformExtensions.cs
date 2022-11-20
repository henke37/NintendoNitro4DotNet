using Henke37.Nitro.Graphics.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.Nitro.Graphics.WinForms {
	public static class WinformExtensions {
		public static Bitmap ToBitmap(this Tile tile, Nitro.Graphics.Palette nitroPal) {
			var bm = new Bitmap(Tile.Width, Tile.Height, PixelFormat.Format8bppIndexed);
			nitroPal.Apply(bm);
			tile.DrawInBitmap(bm);
			return bm;
		}

		public static void DrawInBitmap(this Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false, uint paletteOffset = 0) {
			if(left < 0) throw new ArgumentOutOfRangeException("Left can't be negative!", nameof(left));
			if(top < 0) throw new ArgumentOutOfRangeException("Top can't be negative!", nameof(top));

			switch(bm.PixelFormat) {
				case PixelFormat.Format8bppIndexed:
					DrawTile8Bpp(tile, bm, left, top, flipX, flipY, paletteOffset);
					return;
				case PixelFormat.Format4bppIndexed:
					if(paletteOffset != 0) throw new ArgumentException("4bpp bitmaps do not support palette offsets", nameof(paletteOffset));
					DrawTile4Bpp(tile, bm, left, top, flipX, flipY);
					return;
				default:
					throw new ArgumentException("Unsupported bitmap format", nameof(bm));
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		private static void DrawTile8Bpp(Tile tile, Bitmap bm, int left = 0, int top = 0, bool flipX = false, bool flipY = false, uint paletteOffset = 0) {
			Rectangle rect = new Rectangle {
				X = left,
				Y = top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd = bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			int byteCount = Tile.Width;
			byte[] pixelValues = new byte[byteCount];

			//Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y = 0; y < Tile.Height; ++y) {
				for(int x = 0; x < Tile.Width; ++x) {
					byte pixel = (byte)(tile.TileData[Tile.MirrorX(x, flipX) + Tile.MirrorY(y, flipY) * Tile.Width] + paletteOffset);
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
			if(left % 2 == 1) throw new ArgumentException("Left has to be even!", nameof(left));
			Rectangle rect = new Rectangle {
				X = left,
				Y = top,
				Width = Tile.Width,
				Height = Tile.Height
			};

			var bmd = bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
			int byteCount = Tile.Width / 2;
			byte[] pixelValues = new byte[byteCount];

			//Marshal.Copy(bmd.Scan0, pixelValues, 0, byteCount);

			for(int y = 0; y < Tile.Height; ++y) {
				for(int x = 0; x < Tile.Width; x += 2) {
					int yOff = Tile.MirrorY(y, flipY) * Tile.Width;
					byte pixel1 = tile.TileData[Tile.MirrorX(x, flipX) + yOff];
					byte pixel2 = tile.TileData[Tile.MirrorX(x + 1, flipX) + yOff];
					int index = x / 2;
					pixelValues[index] = (byte)(pixel2 | (pixel1 << 4));
				}

				IntPtr dst = bmd.Scan0 + bmd.Stride * y;
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

			for(int tileY = 0; tileY < Icon.TilesY; ++tileY) {
				for(int tileX = 0; tileX < Icon.TilesX; ++tileX) {
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
				default: throw new ArgumentException("Format not supported", nameof(fmt));
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
			for(int tileY = 0; tileY < tilemap.TilesY; ++tileY) {
				for(int tileX = 0; tileX < tilemap.TilesX; ++tileX) {
					Tilemap.TilemapEntry tileEntry = tilemap.TileMap[tileY, tileX];
					Tile tile = tileSet.Tiles[tileEntry.TileId];
					tile.DrawInBitmap(bm, tileX * Tile.Width, tileY * Tile.Height, tileEntry.XFlip, tileEntry.YFlip, tileEntry.Palette * 16);
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

		public static void DrawInBitmap(this OAMEntry oam, Bitmap bm, Animation.NCER.MappingFormat mapping, GraphicsBank graphics, int xOffset, int yOffset) {
			if(mapping == Animation.NCER.MappingFormat.CM_2D) {
				oam.DrawInBitmap2DMapping(bm, graphics, xOffset, yOffset);
			} else {
				oam.DrawInBitmap1DMapping(bm, graphics, xOffset, yOffset);
			}
		}

		private static void DrawInBitmap1DMapping(this OAMEntry oam, Bitmap bm, GraphicsBank graphics, int xOffset, int yOffset) {
			for(uint tileY = 0; tileY < oam.TilesY; ++tileY) {
				for(uint tileX = 0; tileX < oam.TilesX; ++tileX) {
					uint tileIndex = oam.TileIndex;
					tileIndex += tileX + oam.TilesX * tileY;

					var tile = graphics.Tiles[tileIndex];
					oam.DrawInBitmap(bm, tile, tileX, tileY, xOffset, yOffset);
				}
			}
		}

		public static void DrawInBitmap2DMapping(this OAMEntry oam, Bitmap bm, GraphicsBank graphics, int xOffset, int yOffset) {
			uint baseX = oam.TileIndex % graphics.TilesX;
			uint baseY = oam.TileIndex / graphics.TilesX;
			uint subTileWidth = (graphics.TilesX == 0xFFFF) ? oam.TilesX : graphics.TilesX;

			for(uint tileY = 0; tileY < oam.TilesY; ++tileY) {
				for(uint tileX = 0; tileX < oam.TilesX; ++tileX) {
					uint subTileYIndex = baseY + tileY;
					uint subTileXIndex = baseX + tileX;
					uint tileIndex = subTileXIndex + subTileYIndex * subTileWidth;

					var tile = graphics.Tiles[tileIndex];
					oam.DrawInBitmap(bm, tile, tileX, tileY, xOffset, yOffset);
				}
			}
		}

		private static void DrawInBitmap(this OAMEntry oam, Bitmap bm, Tile tile, uint tileX, uint tileY, int xOffset, int yOffset) {
			int x=oam.X;
			if(oam.XFlip) {
				x += xOffset + Tile.Width * (int)(oam.TilesX - (tileX + 1));
			} else {
				x += xOffset + (int)(Tile.Width * tileX);
			}
			int y=oam.Y;
			if(oam.YFlip) {
				y += yOffset + Tile.Height * (int)(oam.TilesY - (tileY + 1));
			} else {
				y += yOffset + (int)(Tile.Height * tileY);
			}
			tile.DrawInBitmap(bm, x, y, oam.XFlip, oam.YFlip, oam.PaletteIndex * 16);
		}

		public static void DrawInBitmap(this NCER.AnimationCell cell, Bitmap bm, NCER.MappingFormat mapping, GraphicsBank graphics, int xOffset=0, int yOffset=0) {
			foreach(var oam in cell.oams) {
				oam.DrawInBitmap(bm, mapping, graphics, xOffset, yOffset);
			}
		}

		public static Rectangle BoundingBox(this NCER.AnimationCell cell) {
			Rectangle totalBbox = cell.oams[0].BoundingBox();
			for(int oamIndex=1;oamIndex<cell.oams.Count;++oamIndex) {
				var oam = cell.oams[oamIndex];
				totalBbox.ExpandAABB(oam.BoundingBox());
			}

			return totalBbox;
		}

		public static Bitmap DrawOamBoxes(this NCER.AnimationCell cell, Color lineColor) {
			Rectangle bbox = cell.BoundingBox();
			Bitmap bm = new Bitmap(bbox.Width + 1, bbox.Height + 1);
			var g = System.Drawing.Graphics.FromImage(bm);
			var pen = new Pen(lineColor);
			foreach(var oam in cell.oams) {
				g.DrawRectangle(pen, oam.X - bbox.X, oam.Y - bbox.Y, (int)oam.Width, (int)oam.Height);
			}

			return bm;
		}

		public static void ExpandAABB(this ref Rectangle rect1, Rectangle rect2) {
			{
				if(rect1.X > rect2.X) {
					rect1.Width += rect1.X - rect2.X;
					rect1.X = rect2.X;
				}
				int right = rect2.X + rect2.Width;
				if(rect1.Right < right) {
					rect1.Width += right - rect1.Right;
				}
			}

			{
				if(rect1.Y > rect2.Y) {
					rect1.Height += rect1.Y - rect2.Y;
					rect1.Y = rect2.Y;
				}
				int bottom = rect2.Y + rect2.Height;
				if(rect1.Bottom < bottom) {
					rect1.Height += bottom - rect1.Bottom;
				}
			}

		}

		public static Rectangle BoundingBox(this NCER ncer) {
			Rectangle totalBbox;

			totalBbox = ncer.Cells[0].BoundingBox();
			for(int cellIndex = 1; cellIndex < ncer.Cells.Count; ++cellIndex) {
				var cell = ncer.Cells[cellIndex];
				Rectangle cellBbox = cell.BoundingBox();
				totalBbox.ExpandAABB(cellBbox);
			}
			return totalBbox;
		}

		public static Rectangle BoundingBox(this OAMEntry oam) {
			return new Rectangle(
				oam.X,
				oam.Y,
				(int)oam.Width,
				(int)oam.Height
			);
		}
	}
}
