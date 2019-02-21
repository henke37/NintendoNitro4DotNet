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

				stream.Position = cellOffset;
			}
		}

		enum MappingFormat {
			CM_1D_32,
			CM_1D_64,
			CM_1D_128,
			CM_1D_256,
			CM_2D
		}

		public partial class AnimationCell {
		}
	}
}
