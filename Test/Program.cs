using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nitro;
using Nitro.Composer;

namespace Test {
    class Program {
        static void Main(string[] args) {
			NDS nds = new NDS(File.OpenRead(args[0]));
			var sdats=nds.FileSystem.RootDir.FindMatchingFiles("*.nds");
			foreach(var sdatFile in sdats) {
				SDat sdat = SDat.Open(nds.FileSystem.OpenFile(sdatFile));
			}
        }
    }
}
