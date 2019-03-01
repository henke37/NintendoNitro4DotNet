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


			}
		}

		private static int muldiv7(int val, int mul) {
			return mul == 127 ? val : ((val * mul) >> 7);
		}
	}
}