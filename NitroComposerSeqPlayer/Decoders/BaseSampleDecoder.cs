using Nitro.Composer;
using System;

namespace NitroComposerPlayer.Decoders {
	internal abstract class BaseSampleDecoder {

		public static BaseSampleDecoder CreateDecoder(WaveEncoding encoding) {
			switch(encoding) {
				case WaveEncoding.ADPCM:
					return new ADPCMDecoder();
				default:
					throw new NotImplementedException();
			}
		}
	}
}