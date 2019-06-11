using System;

namespace Henke37.Nitro.Composer.Player {
	public class Mixer {

		internal MixerChannel[] channels;

		private const int ChannelCount = 16;

		public Mixer() {
			channels = new MixerChannel[ChannelCount];
			for(int channelIndex=0;channelIndex<ChannelCount;++channelIndex) {
				channels[channelIndex] = new MixerChannel();
			}
		}

		public SamplePair GenerateSamplePair() {
			SamplePair samplePair=new SamplePair();

			foreach(var chan in channels) {
				samplePair+=chan.GenerateSample();
			}

			samplePair.Left = Remap.Clamp(samplePair.Left, -0x80000, 0x7FFF);
			samplePair.Right = Remap.Clamp(samplePair.Right, -0x80000, 0x7FFF);

			return samplePair;
		}


		public int SampleRate {
			set {
				foreach(var chan in channels) {
					chan.SampleRate = value;
				}
			}
		}

	}
}