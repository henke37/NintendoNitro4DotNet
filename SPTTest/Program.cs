using AceAttorney.GK2;
using Nitro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTTest {
	class Program {
		NDS nds;
		MainArchive scriptArchive;

		private void run(string[] args) {
			nds = new NDS(File.OpenRead(args[0]));
			scriptArchive = new MainArchive(nds.FileSystem.OpenFile("jpn/spt.bin"));

			SPT spt = new SPT(scriptArchive.OpenFile(int.Parse(args[1])));
			string contents=spt.OpenSection(int.Parse(args[2]));
		}

		static void Main(string[] args) {
			var p = new Program();
			p.run(args);
		}
	}
}
