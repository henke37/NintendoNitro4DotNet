namespace NitroComposerSeqPlayer {
	internal class MixerChannel {

		MixerChannelMode Mode = MixerChannelMode.Off;

		public enum MixerChannelMode {
			Off,
			Pcm,
			Square,
			Noise
		}

	}
}