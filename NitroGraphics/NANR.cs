using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Graphics {
	public class NANR {
		public NANR(Stream stream) {
			Load(stream);
		}

		private void Load(Stream stream) {
			var sections = new SectionedFile(stream);
			if(sections.MainId != "RNAN") throw new InvalidDataException();
			using(var r=new BinaryReader(sections.Open("KNBA"))) {
				var animCount = r.ReadUInt16();
				var totalFrames = r.ReadUInt16();
				var animOffset = r.ReadUInt32();
				var frameBaseOffset = r.ReadUInt32();
				var positionBaseOffset = r.ReadUInt32();
			}
		}

		public class Animation {
			public List<AnimationFrame> Frames;
			public int LoopStart;
			public AnimationPlaybackMode PlaybackMode;

			public class AnimationFrame {
				public FramePosition Position;
				public int FrameTime;
			}

			public class FramePosition {
				public int CellIndex;
				public object Transform;
			}

			public enum AnimationPlaybackMode {
				Invalid,
				Forward,
				Forward_Loop,
				PingPong_Once,
				PingPong_Loop
			}
		}
	}
}
