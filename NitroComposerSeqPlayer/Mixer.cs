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

		private void GenerateSamplePair() {

			int leftChan = 0;
			int rightChan = 0;

			foreach(var chan in channels) {
				chan.GenerateSample(out int left, out int right);
				leftChan += left;
				rightChan += right;
			}

			leftChan = Remap.Clamp(leftChan, -0x80000, 0x7FFF);
			rightChan = Remap.Clamp(rightChan, -0x80000, 0x7FFF);


		}




	}
}