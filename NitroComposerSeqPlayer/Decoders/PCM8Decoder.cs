using Henke37.IOUtils;
using System.IO;

namespace Henke37.Nitro.Composer.Player.Decoders {
	internal class PCM8Decoder : BaseSampleDecoder {
		public override void Init(BinaryReader reader, uint totalLength, bool loops = false, uint loopLength = 0) {
			this.reader = reader;

			TotalLength = totalLength;
			Loops = loops;
			LoopLength = loopLength;
		}

		internal override int GetSample() {
			reader.Seek((int)samplePosition);
			var b = reader.ReadByte();
			return b | (b<<8);
		}
	}
}