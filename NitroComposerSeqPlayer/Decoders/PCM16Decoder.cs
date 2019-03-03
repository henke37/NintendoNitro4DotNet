using HenkesUtils;
using System.IO;

namespace NitroComposerPlayer.Decoders {
	internal class PCM16Decoder : BaseSampleDecoder {
		public override void Init(BinaryReader reader) {
			this.reader = reader;
		}

		internal override int GetSample() {
			reader.Seek((int)samplePosition * 2);
			return reader.ReadInt16();
		}
	}
}