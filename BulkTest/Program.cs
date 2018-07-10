using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkTest {
	class Program {
		static void Main(string[] args) {
			ScanDir(args[1]);
		}

		private static void ScanDir(string dir) {
			foreach(var file in Directory.EnumerateFiles(dir, "*.nds")) {
				ScanFile(file);
			}
		}

		private static void ScanFile(string file) {
			ScanFile(File.OpenRead(file));
		}

		private static void ScanFile(FileStream fileStream) {
			throw new NotImplementedException();
		}
	}
}
