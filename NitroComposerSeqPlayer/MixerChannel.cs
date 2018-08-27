using System;

namespace NitroComposerSeqPlayer {
	internal class MixerChannel {

		MixerChannelMode Mode = MixerChannelMode.Off;

		public enum MixerChannelMode {
			Off,
			Pcm,
			Pulse,
			Noise
		}

		public void GenerateSamples() {
			switch(Mode) {
				case MixerChannelMode.Off:
				case MixerChannelMode.Pcm:
				case MixerChannelMode.Pulse:
				case MixerChannelMode.Noise:
					throw new NotImplementedException();
			}
		}
	}
}