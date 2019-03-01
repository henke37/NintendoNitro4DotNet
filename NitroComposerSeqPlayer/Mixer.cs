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
				sample = Remap.MulDiv7(sample, chan.VolMul) >> chan.VolShift;
				leftChan += Remap.MulDiv7(sample, 127 - chan.Pan);
				rightChan += Remap.MulDiv7(sample, chan.Pan);
			}

			leftChan = Remap.Clamp(leftChan, -0x80000, 0x7FFF);
			rightChan = Remap.Clamp(rightChan, -0x80000, 0x7FFF);


		}




	}
}