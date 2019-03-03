using System.IO;

namespace NitroComposerPlayer.Decoders {
	internal class PCM8Decoder : BaseSampleDecoder {
		public override void Init(BinaryReader reader) {}

		internal override int GetSample(uint samplePosition) {
			throw new System.NotImplementedException();
		}
	}
}