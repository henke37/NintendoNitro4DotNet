using System;

namespace NitroComposerPlayer {
	internal class Mixer {

		internal MixerChannel[] channels;

		private const int ChannelCount = 16;

		public Mixer() {
			channels = new MixerChannel[ChannelCount];
			for(int channelIndex=0;channelIndex<ChannelCount;++channelIndex) {
				channels[channelIndex] = new MixerChannel();
			}
		}

		private void GeneratSamplePair() {

			int leftChan = 0;
			int rightChan = 0;

			foreach(var chan in channels) {
				int sample=chan.GenerateSample();
				sample = muldiv7(sample, chan.VolMul) >> chan.VolShift;
				leftChan += muldiv7(sample, 127 - chan.Pan);
				rightChan += muldiv7(sample, chan.Pan);
			}

			leftChan = clamp(leftChan, -0x80000, 0x7FFF);
			rightChan = clamp(rightChan, -0x80000, 0x7FFF);


		}

		private int clamp(int x, int bottom, int top) {
			if(x > top) return top;
			if(x < bottom) return bottom;
			return x;
		}

		private static int muldiv7(int val, int mul) {
			return mul == 127 ? val : ((val * mul) >> 7);
		}
	}
}