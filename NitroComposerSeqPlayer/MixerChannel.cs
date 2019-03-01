using System;

namespace NitroComposerPlayer {
	internal class MixerChannel {

		internal MixerChannelMode Mode = MixerChannelMode.Off;

		private ushort noiseState;

		internal int pulseWidth {
			set {
				currentPulseWidthTable = pulseWidthLUT[value];
			}
			get {
				for(int i = 0; i < 8; ++i) {
					if(currentPulseWidthTable == pulseWidthLUT[i]) return i;
				}
				return -1;
			}
		}

		private int[] currentPulseWidthTable;
		private int pulseCounter;

		public enum MixerChannelMode {
			Off,
			Pcm,
			Pulse,
			Noise
		}

		public void GenerateAndAddSamples(int[] outBuf) {
			int bufLen = outBuf.Length;
			switch(Mode) {
				case MixerChannelMode.Off:
					return;
				case MixerChannelMode.Pulse:
					for(int i = 0; i < bufLen;++i) {
						outBuf[i] += GeneratePulse();
					}
					return;
				case MixerChannelMode.Noise:
					for(int i = 0; i < bufLen; ++i) {
						outBuf[i] += GenerateNoise();
					}
					return;
				case MixerChannelMode.Pcm:
					throw new NotImplementedException();
				default:
					throw new InvalidOperationException();
			}
		}

		private int GeneratePCM() {
			throw new NotImplementedException();
		}

		private int GeneratePulse() {
			return currentPulseWidthTable[pulseCounter++ % 8];
		}

		private int GenerateNoise() {
			if((noiseState & 1) == 1) {
				noiseState = (ushort)((noiseState >> 1) ^ 0x6000);
				return -0x7FFF;
			} else {
				noiseState >>= 1;
				return 0x7FFF;
			}
		}

		private static readonly int[][] pulseWidthLUT = new int[][] {
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF}
		};
	}
}