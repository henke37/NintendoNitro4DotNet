using AceAttorney.GK2;
using Henke37.Nitro;
using Henke37.Nitro.Graphics;
using Henke37.Nitro.Graphics.Animation;
using Henke37.Nitro.Graphics.WinForms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace GK2Test {
	public partial class TestForm : Form {

		private NDS nds;

		private AnimationPlayer_WinForms player;

		public TestForm(string[] args) {
			InitializeComponent();

			nds = new NDS(File.OpenRead(args[0]));
			MainArchive mainArchive=new MainArchive(nds.FileSystem.OpenFile(args[1]));

			NCLR nclr=new NCLR(mainArchive.OpenFile(int.Parse(args[2])));
			SubArchive subArchive = new SubArchive(mainArchive.OpenFile(int.Parse(args[3])));
			NCGR ncgr = new NCGR(subArchive.OpenFile(2));
			NANR nanr = new NANR(subArchive.OpenFile(1));
			NCER ncer = new NCER(subArchive.OpenFile(0));

			NANR.Animation anim = nanr.animations[int.Parse(args[4])];

			//imgDisp.Image = cell.DrawOamBoxes(Color.Red);

			player = new AnimationPlayer_WinForms(anim, ncgr, nclr, ncer, imgDisp);
			player.Start();
		}
	}
}
