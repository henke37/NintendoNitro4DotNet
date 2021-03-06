﻿using Henke37.Nitro;
using Henke37.Nitro.Composer;
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
			Console.WriteLine(file);
			ScanFile(File.OpenRead(file));
		}

		private static void ScanFile(Stream fileStream) {
			NDS nds=null;
			try {
				nds = new NDS(fileStream);
			} catch(Exception) {
				Console.WriteLine("NDS parsing failed.");
				return;
			}

			var ser = new SequenceSerializer();

			var sdatFiles=nds.FileSystem.RootDir.FindMatchingFiles("*.sdat");
			foreach(var sdatFile in sdatFiles) {
				SDat sdat;
				try {
					sdat = SDat.Open(nds.FileSystem.OpenFile(sdatFile));
				} catch(InvalidDataException) {
					Console.Out.WriteLine($"{sdatFile.AbsPath} Fail");
					continue;//if this isn't even a valid sdat, no need to try the sequence parser on it
				}
				Console.Out.WriteLine($"{sdatFile.AbsPath} Loaded");

				for(int sequenceIndex=0; sequenceIndex < sdat.sequenceInfo.Count;++ sequenceIndex) {
					if(sdat.sequenceInfo[sequenceIndex] == null) continue;

					string seqName;
					if(sdat.seqSymbols != null && sdat.seqSymbols[sequenceIndex] != null) {
						seqName = $"# {sequenceIndex} {sdat.seqSymbols[sequenceIndex]}";
					} else {
						seqName = $"# {sequenceIndex}";
					}

					SSEQ sseq = null;
					try {
						sseq=sdat.OpenSequence(sequenceIndex);
					} catch(Exception) {
						
						Console.WriteLine($"Sequence {seqName} failed to parse.");
					}

					try {
						ser.Clear();
						ser.Serialize(sseq.sequence);
					} catch(Exception) {
						Console.WriteLine($"Sequence {seqName} failed to serialize.");
					}
				}
			}
		}
	}
}
