using Nitro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Arm7Hasher {
	class Arm7Hasher {
		private Dictionary<string,int> hashes;

		private static readonly SHA1 hasher = SHA1.Create();

		static void Main(string[] args) {
			new Arm7Hasher(args);
		}

		private Arm7Hasher(string[] args) {
			hashes = new Dictionary<string, int>();
			ScanDir(args[0]);
			foreach(var kv in hashes) {
				Console.WriteLine("{0} {1}",kv.Key,kv.Value);
			}
		}

		private void ScanDir(string dir) {
			foreach(var file in Directory.EnumerateFiles(dir, "*.nds")) {
				ScanFile(file);
			}
		}

		private void ScanFile(string file) {
			Console.WriteLine(file);
			ScanFile(File.OpenRead(file));
		}

		private void ScanFile(Stream fileStream) {
			NDS nds = null;
			try {
				nds = new NDS(fileStream);
			} catch(Exception) {
				Console.WriteLine("NDS parsing failed.");
				return;
			}

			var arm7=nds.OpenARM7();
			string hash= BitConverter.ToString(hasher.ComputeHash(arm7));
			if(hashes.ContainsKey(hash)) {
				hashes[hash]++;
			} else {
				hashes.Add(hash, 1);
			}
		}
	}
}
