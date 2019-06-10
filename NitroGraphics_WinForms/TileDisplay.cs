
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Henke37.Nitro.Graphics.WinForms {
	public partial class TileDisplay : Control {

		private Bitmap img;
		private bool imgValid;

		public Tile Tile;
		public Palette Palette;

		public TileDisplay() {
			InitializeComponent();
			imgValid = false;
		}

		protected override void OnPaint(PaintEventArgs pe) {
			var g = pe.Graphics;

			ValidateImg();

			g.DrawImage(img, pe.ClipRectangle);

			base.OnPaint(pe);
		}

		private void ValidateImg() {
			if(imgValid) return;
			RenderImg();
			imgValid = true;
		}

		private void RenderImg() {
			if(img==null) {
				img = Tile.ToBitmap(Palette);
			} else {
				Tile.DrawInBitmap(img, 0, 0);
			}
		}
	}
}
