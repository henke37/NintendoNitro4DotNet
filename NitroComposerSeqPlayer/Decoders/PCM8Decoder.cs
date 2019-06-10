using Henke37.IOUtils;
using System.IO;

namespace NitroComposerPlayer.Decoders {
	internal class PCM8Decoder : BaseSampleDecoder {
		public override void Init(BinaryReader reader) {
			this.reader = reader;
		}

		internal override int GetSample() {
			reader.Seek((int)samplePosition);
			var b = reader.ReadByte();
			return b | (b<<8);
		}
	}
}