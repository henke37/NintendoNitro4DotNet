namespace NitroComposerSeqPlayer {
	internal class MixerChannel {

		MixerChannelMode mode = MixerChannelMode.Off;

		public enum MixerChannelMode {
			Off,
			Pcm,
			Square,
			Noise
		}

	}
}