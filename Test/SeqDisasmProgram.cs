using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nitro;
using Nitro.Composer;

namespace Nitro.Composer.SeqDisasm {
	class SeqDisasmProgram {

		const int ERR_OK = 0;
		const int ERR_NDS_FNF = 1;
		const int ERR_NO_SDAT = 3;
		const int ERR_SEQ_NOT_FOUNT = 5;
		const int ERR_IS_STREAM = 20;
		const int ERR_ARGUMENTS = 90;
		const int ERR_USAGE = 99;

		static int Main(string[] args) {

			if(args.Length == 0) return ListUsage();

			NDS nds;

			try {
				nds = new NDS(File.OpenRead(args[0]));
			} catch(FileNotFoundException) {
				Console.Error.WriteLine("NDS file not found");
				return ERR_NDS_FNF;
			}

			var sdats = nds.FileSystem.RootDir.FindMatchingFiles("*.sdat");

			if(sdats.Count == 0) {
				Console.Error.WriteLine("NDS file not found");
				return ERR_NO_SDAT;
			}

			switch(args.Length) {
				case 1:
					return ListItems(nds, sdats);
				case 2:
					return Disasm(args, nds, sdats);
				default:
					Console.Error.WriteLine("Too many arguments");
					return ERR_ARGUMENTS;
			}
		}

		private static int ListUsage() {
			Console.WriteLine("SDat sequence disassembler");
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine("Disassemble sequence:");
			Console.WriteLine("SeqDisasm game.nds SSEQ_NAME");
			Console.WriteLine("List sequences:");
			Console.WriteLine("SeqDisasm game.nds");
			return ERR_USAGE;
		}

		private static int ListItems(NDS nds, List<FileSystem.File> sdats) {

			foreach(var sdatFile in sdats) {
				SDat sdat = SDat.Open(nds.FileSystem.OpenFile(sdatFile));

				ListItems(sdat);
			}
			return ERR_OK;
		}

		private static void ListItems(SDat sdat) {
			for(int seqIndex = 0; seqIndex < sdat.sequenceInfo.Count; ++seqIndex) {
				if(sdat.seqSymbols != null & sdat.seqSymbols[seqIndex] != null) {
					Console.WriteLine(sdat.seqSymbols[seqIndex]);
				} else {
					Console.WriteLine($"SSEQ #{seqIndex}");
				}
			}

			for(int strmIndex = 0; strmIndex < sdat.streamInfo.Count; ++strmIndex) {
				if(sdat.streamSymbols != null & sdat.streamSymbols[strmIndex] != null) {
					Console.WriteLine(sdat.streamSymbols[strmIndex]);
				} else {
					Console.WriteLine($"STRM #{strmIndex}");
				}
			}
		}

		private static int Disasm(string[] args, NDS nds, List<FileSystem.File> sdats) {
			string name = args[1];

			foreach(var sdatFile in sdats) {
				SDat sdat = SDat.Open(nds.FileSystem.OpenFile(sdatFile));

				try {
					var sseq = sdat.OpenSequence(name);
					var ser = new SequenceSerializer();
					Console.Write(ser.Serialize(sseq.sequence));
					return ERR_OK;
				} catch(FileNotFoundException) {
					//just swallow this one
				}

				try {
					var strm = sdat.OpenStream(name);
					Console.WriteLine("Is stream.");
					return ERR_IS_STREAM;
				} catch(FileNotFoundException) {
					//keep on ignoring missing files
				}
			}

			Console.Error.WriteLine("Sequence not found");
			return ERR_SEQ_NOT_FOUNT;
		}
	}
}
