namespace NitroComposerSeqPlayer {
	internal class ChannelInfo {
		internal int Prio;
		internal int Vol;

		internal MixerChannel mixerChannel;

		internal byte Attack;
		internal byte Decay;
		internal byte Sustain;
		internal byte Release;

		internal uint Duration;

		internal ChannelState state;

		internal int ModulationStartCounter;
		internal int ModulationCounter;

		internal ChannelInfo(MixerChannel mixerChannel) {
			this.mixerChannel = mixerChannel;
		}

		internal enum ChannelState {
			None,
			Start,
			Attack,
			Decay,
			Sustain,
			Release		
		}
	}
}
