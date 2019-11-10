using Henke37.Nitro.Composer;
using System;
using System.IO;

namespace Henke37.Nitro.Composer.Player.Decoders {
	internal abstract class BaseSampleDecoder : BaseGenerator {

		protected uint TotalLength;
		protected uint LoopLength;
		protected bool Loops;

		protected BinaryReader reader;

		public abstract void Init(BinaryReader reader, uint totalLength, bool loops = false, uint loopLength = 0);

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

		public override void IncrementSample() {
			base.IncrementSample();
			if(samplePosition >= TotalLength) {
				if(Loops) {
					while(samplePosition >= TotalLength) {
						samplePosition -= LoopLength;
					}
				} else {
					OnSoundComplete();
				}
			}
		}

	}
}