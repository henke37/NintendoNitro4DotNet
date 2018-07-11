using Nitro;
using NitroComposer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkTest {
	class Program {
		static void Main(string[] args) {
			ScanDir(args[0]);
		}

		private static void ScanDir(string dir) {
			foreach(var file in Directory.EnumerateFiles(dir, "*.nds")) {
				ScanFile(file);
			}
		}

		private static void ScanFile(string file) {
			ScanFile(File.OpenRead(file));
		}

		private static void ScanFile(Stream fileStream) {
			var nds = new NDS(fileStream);

			var sdatFiles=nds.FileSystem.RootDir.FindMatchingFiles("*.sdat");
			foreach(var sdatFile in sdatFiles) {
				var sdat=SDat.Open(nds.FileSystem.OpenFile(sdatFile));
			}
		}
	}
}
