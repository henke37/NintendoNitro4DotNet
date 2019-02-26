using Nitro.Graphics.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Nitro.Graphics.WinForms {
	class AnimationPlayer_WinForms : AnimationRunner {

		private List<Bitmap> bitmaps;

		public AnimationPlayer_WinForms(NANR.Animation animation, NCGR ncgr, NCLR nclr, NCER ncer) : base(animation, ncgr, nclr, ncer) {

			generateBitmaps();
		}

		private void generateBitmaps() {
			Rectangle bbox = ncer.BoundingBox();
			bitmaps = new List<Bitmap>(ncer.Cells.Count);
			foreach(var cell in ncer.Cells) {
				var bm = new Bitmap(bbox.Width, bbox.Height, PixelFormat.Format8bppIndexed);
				nclr.Palette.Apply(bm);
				cell.DrawInBitmap(bm, ncer.Mapping, ncgr, -bbox.X, -bbox.Y);
				bitmaps.Add(bm);
			}
		}

		protected override void drawNewFrame() {
			var bm = bitmaps[currentAnimationFrameIndex];
		}

		protected override void OnAnimationComplete() {
			throw new NotImplementedException();
		}
	}
}
