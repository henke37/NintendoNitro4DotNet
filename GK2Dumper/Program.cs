using AceAttorney.GK2;
using Henke37.Nitro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GK2Dumper {
	class Program {

		private NDS nds;
		private string outputPath;

		static void Main(string[] args) {
			new Program().Run(args);
		}

		public void Run(string[] args) {
			nds = new NDS(File.OpenRead(args[0]));

			outputPath = args[1];

			var archiveList=nds.FileSystem.RootDir.FindMatchingFiles("*.bin");

			foreach(var archiveEntry in archiveList) {
				Console.WriteLine($"Opening archive \"{archiveEntry.AbsPath}\"");
				var archiveStream=nds.FileSystem.OpenFile(archiveEntry);
				var archive = new MainArchive(archiveStream);
				for(var fileIndex=0;fileIndex<archive.FileCount;++fileIndex) {
					try {
						var fileStream = archive.OpenFile(fileIndex);
						var ext = SniffFileExtension(fileStream);
						fileStream.Position = 0;

						if(ext=="bin") {
							bool isArch = SniffArchive(fileStream);
							fileStream.Position = 0;

							if(isArch) {
								Console.WriteLine($"Opening subarch \"{archiveEntry.AbsPath}/{fileIndex}");
								var subArch = new SubArchive(fileStream);
								for(int subFileIndex=0;subFileIndex<subArch.FileCount;++subFileIndex) {
									var subfile = subArch.OpenFile(subFileIndex);
									var subExt = SniffFileExtension(subfile);
									subfile.Position = 0;

									WriteFile(subfile, $"{archiveEntry.AbsPath}/{fileIndex}/{subFileIndex}.{subExt}");
								}
								continue;
							}
						}

						WriteFile(fileStream, $"{archiveEntry.AbsPath}/{fileIndex}.{ext}");
					} catch(Exception err) {
						Console.WriteLine(err.Message);
					}
				}
			}
		}

		private static Regex sigFilter=new Regex("[ a-zA-Z0-9]{4}", RegexOptions.Compiled);

		public string SniffFileExtension(Stream stream) {

			if(stream.Length < 4) return ".bin";

			string sig;

			using(var r = new BinaryReader(stream, Encoding.UTF8, true)) {
				sig = new string(r.ReadChars(4));
			}

			if(!sigFilter.IsMatch(sig)) return "bin";

			sig=sig.Trim(' ');

			switch(sig) {

				case "bmd0":
					return "nsbmd";

				case "btx0":
					return "nsbtx";

				case "bca0":
					return "nsbca";

				default:
					return new string(sig.Reverse().ToArray());
			}
		}

		public bool SniffArchive(Stream stream) {
			using(var r = new BinaryReader(stream, Encoding.UTF8, true)) {
				var firstInt = r.ReadInt32();
				return (firstInt % 4) == 0 && (firstInt / 4) == 3;
			}
		}

		private void WriteFile(Stream stream, string filename) {
			Console.WriteLine($"Extracting \"{filename}\" ({stream.Length} octets)");
			string filePath = $"{outputPath}/{filename}".Replace('/','\\');

			string dirPath = Path.GetDirectoryName(filePath);
			Directory.CreateDirectory(dirPath);

			using(var outFile=File.OpenWrite(filePath)) {
				stream.CopyTo(outFile);
			}
		}
	}
}
