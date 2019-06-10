using Henke37.Nitro.Graphics.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Henke37.Nitro.Graphics.WinForms {
	public class AnimationPlayer_WinForms : AnimationRunner {

		private List<Bitmap> bitmaps;

		public PictureBox PictureBox;
		public Timer Timer;

		public event Action AnimationComplete;

		public AnimationPlayer_WinForms(NANR.Animation animation, NCGR ncgr, NCLR nclr, NCER ncer) : this(animation, ncgr, nclr, ncer, new PictureBox()) { }

		public AnimationPlayer_WinForms(NANR.Animation animation, NCGR ncgr, NCLR nclr, NCER ncer, PictureBox pb) : base(animation, ncgr, nclr, ncer) {
			PictureBox = pb;
			generateBitmaps();

			Timer = new Timer();
			Timer.Interval = 1000 / 60;
			Timer.Tick += Timer_Tick;
		}

		private void Timer_Tick(object sender, EventArgs e) {
			tick();
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

			PictureBox.Width = bbox.Width;
			PictureBox.Height = bbox.Height;
		}

		public void Start() {
			currentAnimationFrameIndex = 0;
			Timer.Start();
		}

		protected override void drawNewFrame() {
			var animFrame = currentAnimationFrame;
			var bm = bitmaps[animFrame.Position.CellIndex];
			PictureBox.Image = bm;
		}

		protected override void OnAnimationComplete() {
			Timer.Stop();
			AnimationComplete?.Invoke();
		}
	}
}
