using System;

namespace NitroComposerSeqPlayer {
	internal class ChannelInfo {

		internal TrackPlayer Track;

		internal int Prio;
		internal int Vol;

		internal byte Note;
		internal uint Velocity;

		internal MixerChannel mixerChannel;

		internal byte AttackLevel;
		internal byte DecayRate;
		internal byte SustainLevel;
		internal byte ReleaseRate;

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

		internal void Release() {
			state = ChannelState.Release;
			Prio = 1;
		}
	}
}
