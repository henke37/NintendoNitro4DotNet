using Nitro;
using Nitro.Graphics;
using Nitro.Graphics.WinForms;
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

			iconDisplay.Image = nds.Banner.Icon().ToBitmap();
		}
	}
}
