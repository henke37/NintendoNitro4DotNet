using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Graphics {
	public static class Extensions {
		public static BGR555 ReadBGR555(this BinaryReader reader) {
			var u = reader.ReadUInt16();
			return (BGR555)(u);
		}

		public static Tile ReadTile(this BinaryReader reader, TextureFormat format) {
			return new Tile(reader, format);
		}
	}
}
