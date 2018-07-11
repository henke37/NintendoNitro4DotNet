using System;
using System.Collections.Generic;
using System.IO;
using HenkesUtils;

namespace Nitro {
	public class FileSystem {

		private Directory RootDir;

		public FileSystem(Stream FNTStream, Stream FATStream) {
			if(FNTStream == null) {
				throw new System.ArgumentNullException(nameof(FNTStream));
			}

			if(FATStream == null) {
				throw new System.ArgumentNullException(nameof(FATStream));
			}

			using(var r=new BinaryReader(FATStream)) {
				LoadFAT(r);
			}

			using(var r = new BinaryReader(FNTStream)) {
				LoadFNT(r);
			}
		}

		private void LoadFAT(BinaryReader r) {
		}

		private void LoadFNT(BinaryReader r) {

			//root entry smuggles the total number of directories
			DirIndexEntry rootDirEntry = new DirIndexEntry(r.ReadUInt32(),r.ReadUInt16(),0);
			RootDir = new Directory("$ROOT", null);
			rootDirEntry.dir = RootDir;
			int dirCount = r.ReadUInt16();
			DirIndexEntry[] dirEntries = new DirIndexEntry[dirCount];
			dirEntries[0] = rootDirEntry;

			for(int dirIndex=1;dirIndex<dirCount;++dirIndex) {
				dirEntries[dirIndex] =new DirIndexEntry(r.ReadUInt32(), r.ReadUInt16(), r.ReadUInt16());
			}

			foreach(var dirEntry in dirEntries) {
				int fileId = dirEntry.firstFileId;
				r.Seek((int)dirEntry.offset);

				for(; ;) {
					byte type = r.ReadByte();
					if(type == 0) break;

					string name = r.ReadUTFString(type & 0x7F, false);
					if((type & 0x80)==0x80) {
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

		private class AbstractFile {
			public string Name;
			public Directory Parent;

			public AbstractFile(string Name, Directory Parent) {
				this.Name = Name;
				this.Parent = Parent;
			}
		}

		private class Directory : AbstractFile {
			public List<AbstractFile> Files;

			public Directory(string Name, Directory Parent) : base (Name, Parent) {
				Files = new List<AbstractFile>();
			}
		}

		private class File : AbstractFile {
			int FatIndex;
			public File(string Name, Directory Parent, int FatIndex) : base(Name, Parent) {
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