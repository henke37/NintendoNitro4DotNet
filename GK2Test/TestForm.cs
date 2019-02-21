using AceAttorney.GK2;
using Nitro;
using Nitro.Graphics;
using System.IO;
using System.Windows.Forms;

namespace GK2Test {
	public partial class TestForm : Form {

		private NDS nds;

		public TestForm(string[] args) {
			nds = new NDS(File.OpenRead(args[0]));
			MainArchive mainArchive=new MainArchive(nds.FileSystem.OpenFile(args[1]));

			NCLR nclr=new NCLR(mainArchive.OpenFile(5));
			SubArchive subArchive = new SubArchive(mainArchive.OpenFile(4));
			NCGR ncgr = new NCGR(subArchive.OpenFile(2));
		}
	}
}
