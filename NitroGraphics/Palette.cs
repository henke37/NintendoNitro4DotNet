using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Graphics {
	public class Palette {
		public List<BGR555> Colors;

		public Palette() {
			Colors = new List<BGR555>();
		}

		public Palette(IEnumerable<BGR555> palette) {
			Colors = new List<BGR555>(palette);
		}
	}
}
