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
        static int Main(string[] args) {
			NDS nds = new NDS(File.OpenRead(args[0]));
			var sdats=nds.FileSystem.RootDir.FindMatchingFiles("*.sdat");

			string name = args[1];

			foreach(var sdatFile in sdats) {
				SDat sdat = SDat.Open(nds.FileSystem.OpenFile(sdatFile));

				try {
					var sseq = sdat.OpenSequence(name);
					var ser = new SequenceSerializer();
					Console.Write(ser.Serialize(sseq.sequence));
					return 0;
				} catch (FileNotFoundException) {
					//just swallow this one
				}

				try {
					var strm = sdat.OpenStream(name);
					return 0;
				} catch(FileNotFoundException) {
					//keep on ignoring missing files
				}
			}

			return 1;
        }
    }
}
