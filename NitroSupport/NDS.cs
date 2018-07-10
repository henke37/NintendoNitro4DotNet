using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HenkesUtils;

namespace Nitro {
	public class NDS {
		private Stream rootStream;

		public string GameTitle;
		public string GameCode;
		public string MakerCode;
		private UnitCodeEnum UnitCode;
		private RegionEnum Region;
		private byte RomVersion;
		private RomFlagsEnum RomFlags;
		private ExecutableInfo ARM9;
		private ExecutableInfo ARM7;
		public FileSystem FileSystem;
		public Banner Banner;

		public NDS(Stream _root) {
			rootStream = _root;
			Load();
		}

		private void Load() {
			using(var r=new BinaryReader(new SubStream(rootStream,0))) {
				GameTitle=r.ReadUTFString(12);
				GameCode = r.Read4C();
				MakerCode = r.ReadUTFString(2);
				UnitCode = (UnitCodeEnum)r.ReadByte();
				r.Skip(1 + 1 + 7 + 1);
				Region = (RegionEnum)r.ReadByte();
				RomVersion = r.ReadByte();
				RomFlags = (RomFlagsEnum)r.ReadByte();

				ARM9 = new ExecutableInfo(r.ReadUInt32(), r.ReadUInt32(), r.ReadUInt32(), r.ReadUInt32());
				ARM7 = new ExecutableInfo(r.ReadUInt32(), r.ReadUInt32(), r.ReadUInt32(), r.ReadUInt32());

				{
					uint FNTOffset = r.ReadUInt32();
					uint FNTSize = r.ReadUInt32();
					uint FATOffset = r.ReadUInt32();
					uint FATSize = r.ReadUInt32();

					if(FNTSize!=0 && FATSize!=0) {
						FileSystem = new FileSystem(
							new SubStream(rootStream,FNTOffset,FNTSize),
							new SubStream(rootStream,FATOffset,FATSize)
						);
					}
				}

				{
					uint ARM9OverlayOffset = r.ReadUInt32();
					uint ARM9OverlaySize = r.ReadUInt32();
					uint ARM7OverlayOffset = r.ReadUInt32();
					uint ARM7OverlaySize = r.ReadUInt32();
				}

				r.Skip(4 * 2);

				{
					uint bannerOffset = r.ReadUInt32();
					Banner = new Banner(new SubStream(rootStream,bannerOffset));
				}

				UInt16 SecureAreaChecksum = r.ReadUInt16();
				UInt16 SecureAreaDelay = r.ReadUInt16();

				r.Skip(4 * 2);
			}
		}

		private enum UnitCodeEnum {
			NDS = 0,
			NDS_DSI = 2,
			DSI = 3
		}

		private enum RegionEnum {
			Normal=0,
			China=0x80,
			Korea=0x40
		}

		[Flags]
		private enum RomFlagsEnum {
			AutoBoot=2
		}

		private struct ExecutableInfo {
			private uint RomOffset;
			private uint Entrypoint;
			private uint RamOffset;
			private uint Size;

			public ExecutableInfo(uint RomOffset, uint Entrypoint, uint RamOffset, uint Size) {
				this.RomOffset = RomOffset;
				this.Entrypoint = Entrypoint;
				this.RamOffset = RamOffset;
				this.Size = Size;
			}
		}
	}
}
