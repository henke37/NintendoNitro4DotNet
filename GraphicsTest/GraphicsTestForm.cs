using Nitro;
using Nitro.Graphics;
using Nitro.Graphics.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GraphicsTest {
	public partial class GraphicsTestForm : Form {

		NDS nds;

		NCLR nclr;
		NCGR ncgr;
		NSCR nscr;

		public GraphicsTestForm(string[] args) {
			InitializeComponent();

			nds = new NDS(File.OpenRead(args[0]));

			nclr = new NCLR(nds.FileSystem.OpenFile(@"data/data0/BG1.NCLR"));
			ncgr = new NCGR(nds.FileSystem.OpenFile(@"data/data0/BG1.NCGR"));
			nscr = new NSCR(nds.FileSystem.OpenFile(@"data/data0/BG1.NSCR"));

			Bitmap bitmap=nscr.ToBitmap(ncgr,nclr.Palette);

			ImgDisp.Image=bitmap;
		}
	}
}
