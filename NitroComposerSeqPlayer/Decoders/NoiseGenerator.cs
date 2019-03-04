using System;
using System.Collections.Generic;

namespace NitroComposerPlayer.Decoders {
	class NoiseGenerator : BaseGenerator {

		private uint LastPRNGClock;

		private ushort noiseState;

		public NoiseGenerator() {
			noiseState = 0x7FFF;
		}

		internal override int GetSample() {
			int sample;
			do {
				sample = ClockNoise();
				++LastPRNGClock;
			} while(LastPRNGClock < samplePosition);
			return sample;
		}

		private int ClockNoise() {
			if((noiseState & 1) == 1) {
				noiseState = (ushort)((noiseState >> 1) ^ 0x6000);
				return -0x7FFF;
			} else {
				noiseState >>= 1;
				return 0x7FFF;
			}
		}
	}
}
