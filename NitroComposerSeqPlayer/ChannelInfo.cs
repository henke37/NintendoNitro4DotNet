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
			None=0,
			Start=1,
			Attack=2,
			Decay=3,
			Sustain=4,
			Release=5		
		}

		internal void Release() {
			state = ChannelState.Release;
			Prio = 1;
		}

		internal void UpdateTrackData() {

			var trackFlags = Track.updateFlags;

			if(trackFlags == 0) return;

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Length)) {
				if(state>ChannelState.Start) {
					if(state<ChannelState.Release && this.Duration>0) {
						Release();
					}
					//TODO: sweep thing
				}
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Volume)) {
				UpdateVolume();
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Pan)) {
				//UpdatePan();
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Timer)) {
				//UpdateTimer();
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Modulation)) {
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
