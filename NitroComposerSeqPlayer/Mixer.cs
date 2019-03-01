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

		public int[] GenerateSamples(int sampleCount) {
			int[] outBuff = new int[sampleCount];
			GenerateAndAddSamples(outBuff);
			return outBuff;
		}

		private void GenerateAndAddSamples(int[] outBuff) {
			foreach(var chan in channels) {
				chan.GenerateAndAddSamples(outBuff);
			}
		}
	}
}