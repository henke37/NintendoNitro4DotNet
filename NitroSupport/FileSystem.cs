using System;
using System.Collections.Generic;
using System.IO;
using HenkesUtils;
using Like = Microsoft.VisualBasic.CompilerServices.LikeOperator;

namespace Nitro {
	public class FileSystem {

		public Directory RootDir { get; private set;  }

		private List<FATEntry> FAT;

		private Stream DataStream;

		public FileSystem(Stream FNTStream, Stream FATStream, Stream DataStream) {
			if(FNTStream == null) {
				throw new ArgumentNullException(nameof(FNTStream));
			}

			if(FATStream == null) {
				throw new ArgumentNullException(nameof(FATStream));
			}

			this.DataStream = DataStream;

			using(var r = new BinaryReader(FATStream)) {
				LoadFAT(r);
			}

			using(var r = new BinaryReader(FNTStream)) {
				LoadFNT(r);
			}
		}

		private File resolveFilename(string fileName) {
			var parts = fileName.Split('/');

			var curDir = RootDir;

			foreach(var part in parts) {
				var absFile = curDir.FindFile(part);
				{
					var file = absFile as File;
					if(file != null) return file;
				}
				curDir = (Directory)absFile;
			}
			throw new FileNotFoundException();
		}

		public Stream OpenFile(string fileName) {
			File file = resolveFilename(fileName);
			return OpenFile(file);
		}

		public Stream OpenFile(File file) {
			return OpenFile(file.FatIndex);
		}

		private Stream OpenFile(int index) {
			FATEntry fatEntry = FAT[index];
			uint len = fatEntry.End - fatEntry.Start;
			return new SubStream(DataStream, fatEntry.Start, len);
		}

		private void LoadFAT(BinaryReader r) {
			int entryCount = (int)(r.BytesLeft() / 8);
			FAT = new List<FATEntry>(entryCount);
			for(var entryIndex = 0; entryIndex < entryCount; ++entryIndex) {
				FAT.Add(new FATEntry(r.ReadUInt32(), r.ReadUInt32()));
			}
		}

		private struct FATEntry {
			public UInt32 Start;
			public UInt32 End;

			public FATEntry(uint Start, uint End) {
				this.Start = Start;
				this.End = End;
			}
		}

		private void LoadFNT(BinaryReader r) {

			//root entry smuggles the total number of directories
			DirIndexEntry rootDirEntry = new DirIndexEntry(r.ReadUInt32(), r.ReadUInt16(), 0);
			RootDir = new Directory("$ROOT", null);
			rootDirEntry.dir = RootDir;
			int dirCount = r.ReadUInt16();
			DirIndexEntry[] dirEntries = new DirIndexEntry[dirCount];
			dirEntries[0] = rootDirEntry;

			for(int dirIndex = 1; dirIndex < dirCount; ++dirIndex) {
				dirEntries[dirIndex] = new DirIndexEntry(r.ReadUInt32(), r.ReadUInt16(), r.ReadUInt16());
			}

			foreach(var dirEntry in dirEntries) {
				int fileId = dirEntry.firstFileId;
				r.Seek((int)dirEntry.offset);

				for(; ; ) {
					byte type = r.ReadByte();
					if(type == 0) break;

					string name = r.ReadUTFString(type & 0x7F, false);
					if((type & 0x80) == 0x80) {
						var subDir = new Directory(name, dirEntry.dir);
						dirEntry.dir.Files.Add(subDir);
						int subDirIndex = r.ReadUInt16() & 0x0FFF;
						dirEntries[subDirIndex].dir = subDir;
					} else {
						var subFile = new File(name, dirEntry.dir, fileId++);
						dirEntry.dir.Files.Add(subFile);
					}
				}
			}
		}

		public class AbstractFile {
			public string Name;
			public Directory Parent;

			public AbstractFile(string Name, Directory Parent) {
				this.Name = Name;
				this.Parent = Parent;
			}
		}

		public class Directory : AbstractFile {
			public List<AbstractFile> Files;

			public Directory(string Name, Directory Parent) : base(Name, Parent) {
				Files = new List<AbstractFile>();
			}

			public AbstractFile FindFile(string fileName) {
				foreach(var file in Files) {
					if(file.Name != fileName) continue;
					return file;
				}
				throw new FileNotFoundException();
			}

			public List<File> FindMatchingFiles(string pattern) {
				var l = new List<File>();
				FindMatchingFiles(l, pattern);
				return l;
			}

			private void FindMatchingFiles(List<File> l, string pattern) {
				foreach(var absFile in Files) {
					{
						var dir = absFile as Directory;
						if(dir != null) {
							dir.FindMatchingFiles(l, pattern);
							continue;
						}
					}
					var file = (File)absFile;
					if(!Like.LikeString(file.Name,pattern,Microsoft.VisualBasic.CompareMethod.Text)) continue;
					l.Add(file);
				}
			}
		}

		public class File : AbstractFile {
			internal int FatIndex;
			internal File(string Name, Directory Parent, int FatIndex) : base(Name, Parent) {
				this.FatIndex = FatIndex;
			}
		}

		private class DirIndexEntry {
			public uint offset;
			public int firstFileId;
			public int parent;
			public Directory dir;

			internal DirIndexEntry(uint offset, int firstFileId, int parent) {
				this.offset = offset;
				this.firstFileId = firstFileId;
				this.parent = parent;
			}
		}
	}
}