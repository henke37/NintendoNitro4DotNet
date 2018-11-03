using Nitro.Composer;
using System;
using System.IO;

namespace NitroComposerPlayer.Decoders {
	internal abstract class BaseSampleDecoder {

		public abstract void Init(BinaryReader reader);

		public static BaseSampleDecoder CreateDecoder(WaveEncoding encoding) {
			switch(encoding) {
				case WaveEncoding.PCM8:
					return new PCM8Decoder();
				case WaveEncoding.PCM16:
					return new PCM16Decoder();
				case WaveEncoding.ADPCM:
					return new ADPCMDecoder();
				case WaveEncoding.GEN:
					throw new ArgumentException();
				default:
					throw new NotImplementedException();
			}
		}
	}
}