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

		private CacheStalenessFlags staleFlags;

		internal ChannelInfo(MixerChannel mixerChannel) {
			this.mixerChannel = mixerChannel;
		}

		internal enum ChannelState {
			None=0,
			Start=1,
			Attack=2,
			Decay=3,
			Sustain=4,
			Release=5		
		}

		[Flags]
		private enum CacheStalenessFlags {
			Volume=1,
			Pan=2,
			Timer=4,
			Modulation=8,
			Length=16
		}

		internal void Release() {
			state = ChannelState.Release;
			Prio = 1;
		}

		internal void UpdateCachedData() {
			if(staleFlags == 0) return;

			if(staleFlags.HasFlag(CacheStalenessFlags.Length)) {
				if(state>ChannelState.Start) {
					if(state<ChannelState.Release && this.Duration>0) {
						Release();
					}
					//TODO: sweep thing
				}
			}

			if(staleFlags.HasFlag(CacheStalenessFlags.Volume)) {
				UpdateVolume();
			}

			if(staleFlags.HasFlag(CacheStalenessFlags.Pan)) {
				//UpdatePan();
			}

			if(staleFlags.HasFlag(CacheStalenessFlags.Timer)) {
				//UpdateTimer();
			}

			if(staleFlags.HasFlag(CacheStalenessFlags.Modulation)) {
				//UpdateModulation();
			}
		}

		private void UpdateVolume() {
			int finalVol = Track.sequencePlayer.MasterVolume;
			finalVol += Track.sequencePlayer.seqInfo.vol;
			finalVol += ConvertLevel(Track.Volume);
			finalVol += ConvertLevel(Track.Expression);

		}

		private int ConvertLevel(byte volume) {
			throw new NotImplementedException();
		}

		internal void Update() {

		}
	}
}
