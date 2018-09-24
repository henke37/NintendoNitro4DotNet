using HenkesUtils;
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
			using(var r = new BinaryReader(sections.Open("KNBA"))) {
				LoadKNBA(r);
			}
		}

		private void LoadKNBA(BinaryReader r) {
			var animCount = r.ReadUInt16();
			var totalFrames = r.ReadUInt16();
			var animOffset = r.ReadUInt32();
			var frameBaseOffset = r.ReadUInt32();
			var positionBaseOffset = r.ReadUInt32();

			var positionReader = new PositionReader(new SubStream(r.BaseStream, positionBaseOffset));
			using(var animationReader = new BinaryReader(new SubStream(r.BaseStream, animOffset))) {
				for(var animIndex = 0; animIndex < animCount; ++animIndex) {
					var anim = new Animation();
					var animationFrames = animationReader.ReadUInt16();
					anim.LoopStart = animationReader.ReadUInt16();
					var positionType = (Animation.FramePosition.PositionType)animationReader.ReadUInt32();
					anim.PlaybackMode = (Animation.AnimationPlaybackMode)animationReader.ReadUInt32();
					var frameOffset = animationReader.ReadUInt32();

					using(var frameReader = new BinaryReader(new SubStream(r.BaseStream, frameBaseOffset + frameOffset))) {
						anim.Frames = readFrames(animationFrames, positionType, frameReader, positionReader);
					}
				}
			}
		}

		private class PositionReader {
			BinaryReader reader;
			Dictionary<int, Animation.FramePosition> positions = new Dictionary<int, Animation.FramePosition>();

			public PositionReader(Stream stream) {
				reader = new BinaryReader(stream);
			}

			public Animation.FramePosition ReadPosition(int positionOffset, Animation.FramePosition.PositionType positionType) {
				Animation.FramePosition Position;
				if(positions.TryGetValue(positionOffset, out Position)) {
					return Position;
				}

				reader.Seek(positionOffset);
				var position = new Animation.FramePosition();
				position.CellIndex = reader.ReadUInt16();

				switch(positionType) {
					case Animation.FramePosition.PositionType.Index:
						break;
					case Animation.FramePosition.PositionType.IndexRotationScaleTranslation:
						throw new NotImplementedException();
					case Animation.FramePosition.PositionType.IndexTranslation:
						reader.Skip(2);
						var tx = reader.ReadInt16();
						var ty = reader.ReadInt16();
						break;
				}
				positions.Add(positionOffset, position);
				return position;
			}
		}

		private List<Animation.AnimationFrame> readFrames(ushort totalFrames, Animation.FramePosition.PositionType positionType, BinaryReader frameReader, PositionReader positionReader) {

			List<Animation.AnimationFrame> Frames = new List<Animation.AnimationFrame>();
			for(int frameIndex = 0; frameIndex < totalFrames; ++frameIndex) {
				var frame = new Animation.AnimationFrame();
				int positionOffset = frameReader.ReadInt32();
				frame.Position = positionReader.ReadPosition(positionOffset, positionType);

				Frames.Add(frame);
			}
			return Frames;
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

				internal enum PositionType {
					Index = 0,
					IndexRotationScaleTranslation = 1,
					IndexTranslation = 2
				}
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