using System;
using System.IO;
using Nitro.Composer;

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

		private ushort _timer;
		private uint sampleIncrease;

		public ushort Timer {
			get => _timer;
			set {
				_timer = value;

				sampleIncrease = (uint)(ARM7_CLOCK / (sampleRate * 2) / (0x10000 - value));
			}
		}

		private uint samplePosition;
		internal uint TotalLength;
		internal uint LoopLength;
		internal bool Loops;

		private int[] currentPulseWidthTable;
		private int pulseCounter;

		internal int Pan;
		internal int VolMul;
		internal int VolShift;

		public enum MixerChannelMode {
			Off,
			Pcm,
			Pulse,
			Noise
		}

		public int GenerateBaseSample() {
			switch(Mode) {
				case MixerChannelMode.Off:
					return 0;
				case MixerChannelMode.Pulse:
					return GeneratePulse();
				case MixerChannelMode.Noise:
					return GenerateNoise();
				case MixerChannelMode.Pcm:
					return GeneratePCM();
				default:
					throw new InvalidOperationException();
			}
		}

		public void GenerateSample(out int leftChan, out int rightChan) {
			int sample = GenerateBaseSample();
			sample = Remap.MulDiv7(sample, VolMul) >> VolShift;
			leftChan = Remap.MulDiv7(sample, 127 - Pan);
			rightChan = Remap.MulDiv7(sample, Pan);
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

		private void IncrementSample() {
			samplePosition += sampleIncrease;
			if(Mode == MixerChannelMode.Pcm && samplePosition >= TotalLength) {
				if(Loops) {
					while(samplePosition >= TotalLength) {
						samplePosition -= LoopLength;
					}
				} else {
					Mode = MixerChannelMode.Off;

				}
			}
		}

		internal void SetSampleData(Stream dataStream, WaveEncoding encoding) {
			throw new NotImplementedException();
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
		private readonly int sampleRate;
		private const int ARM7_CLOCK = 33513982;

	}
}