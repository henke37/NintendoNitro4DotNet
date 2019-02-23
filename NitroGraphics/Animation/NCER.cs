using HenkesUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Graphics.Animation {
	public class NCER {
		public List<AnimationCell> Cells;

		public List<string> Labels;
		private MappingFormat Mapping;

		public NCER(Stream stream) {
			var sections = new SectionedFile(stream);
			if(sections.MainId != "RECN") throw new InvalidDataException("Bad signature");

			parseCellBank(sections.Open("KBEC"));
		}

		private void parseCellBank(Stream stream) {
			using(var r = new BinaryReader(stream)) {
				int cellCount = r.ReadUInt16();
				var attributes = r.ReadUInt16();
				Cells = new List<AnimationCell>(cellCount);

				uint cellOffset = r.ReadUInt32();
				Mapping = (MappingFormat)r.ReadUInt32();
				//3 junk pointers that aren't used
				stream.Position = cellOffset;

				long cellDataSize = (attributes == 1) ? 16 : 8;

				long oamStart = cellOffset + cellDataSize * cellCount;

				for(int cellIndex=0;cellIndex<cellCount;++cellIndex) {
					var cell = new AnimationCell();

					int numOAMS = r.ReadUInt16();
					r.Skip(2);//attributes
					long oamOffset = r.ReadUInt32();

					{
						long curPos = stream.Position;

						stream.Position = oamStart + oamOffset;
						var oams = new List<OAMEntry>(numOAMS);
						for(int oamIndex = 0; oamIndex < numOAMS; ++oamIndex) {
							var oam = new OAMEntry();
							oam.Load(r, ((int)Mapping) & 3, 0);
							oams.Add(oam);
						}
						cell.oams = oams;
						Cells.Add(cell);

						stream.Position = curPos;
					}

					if(attributes==1) {
						r.Skip(8);
					}
				}
			}
		}

		enum MappingFormat {
			CM_1D_32=0,
			CM_1D_64=1,
			CM_1D_128=2,
			CM_1D_256=3,
			CM_2D=4
		}

		public class AnimationCell {
			public List<OAMEntry> oams;
		}
	}
}
