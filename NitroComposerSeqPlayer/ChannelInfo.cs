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

		private ChannelUpdateFlags updateFlags;

		internal ChannelInfo(MixerChannel mixerChannel) {
			this.mixerChannel = mixerChannel;
		}

		internal enum ChannelState {
			None = 0,
			Start = 1,
			Attack = 2,
			Decay = 3,
			Sustain = 4,
			Release = 5
		}

		internal void Release() {
			state = ChannelState.Release;
			Prio = 1;
		}

		internal void UpdateTrackData() {

			var trackFlags = Track.updateFlags;

			if(trackFlags == 0) return;

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Length)) {
				if(state > ChannelState.Start) {
					if(state < ChannelState.Release && this.Duration > 0) {
						Release();
					}
					//TODO: sweep thing
				}
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Volume)) {
				UpdateVolume();
				updateFlags |= ChannelUpdateFlags.Volume;
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Pan)) {
				//UpdatePan();
				updateFlags |= ChannelUpdateFlags.Pan;
			}

			if(trackFlags.HasFlag(TrackPlayer.TrackUpdateFlags.Timer)) {
				//UpdateTimer();
				updateFlags |= ChannelUpdateFlags.Timer;
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

			bool bNotInSustain = state != ChannelState.Sustain;
			bool bInStart = state == ChannelState.Start;
			//bool bPitchSweep = this->sweepPitch && this->sweepLen && this->sweepCnt <= this->sweepLen;
			bool bModulation = Track.ModulationDepth != 0;
			bool bVolNeedUpdate = updateFlags.HasFlag(ChannelUpdateFlags.Volume) || bNotInSustain;
			bool bPanNeedUpdate = updateFlags.HasFlag(ChannelUpdateFlags.Pan) || bInStart;
			bool bTmrNeedUpdate = updateFlags.HasFlag(ChannelUpdateFlags.Timer) || bInStart /* || bPitchSweep*/;

			switch(state) {
				case ChannelState.None:
					return;
				case ChannelState.Start:
					state = ChannelState.Attack;
					goto case ChannelState.Attack;
				case ChannelState.Attack:
					break;
				case ChannelState.Release:
					break;
			}

			if(bModulation && ModulationStartCounter < Track.ModulationDelay) {
				++ModulationStartCounter;
				bModulation = false;
			}

			if(bModulation) {
				switch(Track.ModulationType) {
					case TrackPlayer.ModulationTypeEnum.Pitch:
						bTmrNeedUpdate = true;
						break;
					case TrackPlayer.ModulationTypeEnum.Volume:
						bVolNeedUpdate = true;
						break;
					case TrackPlayer.ModulationTypeEnum.Pan:
						bPanNeedUpdate = true;
						break;
				}
			}
		}

		[Flags]
		private enum ChannelUpdateFlags {
			Volume,
			Pan,
			Timer
		}

	}
}
