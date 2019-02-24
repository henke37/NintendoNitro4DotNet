using AceAttorney.GK2;
using Nitro;
using Nitro.Graphics;
using Nitro.Graphics.Animation;
using Nitro.Graphics.WinForms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace GK2Test {
	public partial class TestForm : Form {

		private NDS nds;

		public TestForm(string[] args) {
			InitializeComponent();

			nds = new NDS(File.OpenRead(args[0]));
			MainArchive mainArchive=new MainArchive(nds.FileSystem.OpenFile(args[1]));

			NCLR nclr=new NCLR(mainArchive.OpenFile(int.Parse(args[2])));
			SubArchive subArchive = new SubArchive(mainArchive.OpenFile(int.Parse(args[3])));
			NCGR ncgr = new NCGR(subArchive.OpenFile(2));
			NANR nanr = new NANR(subArchive.OpenFile(1));
			NCER ncer = new NCER(subArchive.OpenFile(0));

			NCER.AnimationCell cell = ncer.Cells[0];
			Rectangle bbox=cell.BoundingBox();

			//imgDisp.Image = cell.DrawOamBoxes(Color.Red);

			var bm = new Bitmap(bbox.Width, bbox.Height, PixelFormat.Format8bppIndexed);
			nclr.Palette.Apply(bm);
			cell.DrawInBitmap(bm, ncer.Mapping, ncgr, -bbox.X, -bbox.Y);
			imgDisp.Image = bm;

		}
	}
}
