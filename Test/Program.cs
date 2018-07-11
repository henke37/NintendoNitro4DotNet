using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nitro;
using NitroComposer;

namespace Test {
    class Program {
        static void Main(string[] args) {
			NDS nds = new NDS(File.OpenRead(args[0]));
			SDat sdat = SDat.Open(nds.FileSystem.OpenFile("sound_data.sdat"));
        }
    }
}
