using Nitro;
using Henke37.Nitro.Graphics;
using Henke37.Nitro.Graphics.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconTest {
	public partial class IconTest : Form {

		NDS nds;

		public IconTest(string romName) {
			InitializeComponent();

			nds = new NDS(File.OpenRead(romName));

			Bitmap bm= nds.Banner.Icon().ToBitmap();

			iconDisplay.Image = bm;
		}
	}
}
