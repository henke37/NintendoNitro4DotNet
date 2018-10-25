﻿using System;
using Nitro.Composer.Instruments;

namespace NitroComposerSeqPlayer {
	internal class ChannelInfo {

		internal TrackPlayer Track;

		internal int Prio;
		internal int Vol;

		internal byte Note;
		internal uint Velocity;

		internal MixerChannel mixerChannel;

		internal int AttackLevel;
		internal int DecayRate;
		internal int SustainLevel;
		internal int ReleaseRate;

		private const int EnvelopeStartLevel = -723 << 7;
		private int EnvelopeLevel;

		internal uint Duration;
		internal ChannelState state;

		internal int ModulationStartCounter;
		internal int ModulationCounter;

		private bool ManualSweep;
		private uint SweepLength;
		private uint SweepCounter;
		private ushort SweepPitch;

		private ChannelUpdateFlags updateFlags;
		private BaseLeafInstrument instrument;

		internal BaseLeafInstrument Instrument {
			get => instrument; set {
				instrument = value;
				SetupMixerChannelForNote();
			}
		}


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

		private void Kill() {
			state = ChannelState.None;
			Track = null;
			Prio = 0;
			Vol = 0;
			Duration = 0;
		}

		internal void UpdateTrackData() {
			if(Track == null) return;

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

		private void UpdatePorta() {
			ManualSweep = false;
			SweepPitch = Track.SweepPitch;
			SweepCounter = 0;
			if(!Track.portamentoEnabled) {
				SweepLength = 0;
				return;
			}

			ushort diff = (ushort)(Track.portamentoKey - Note << 22);
			SweepPitch += diff;

			if(Track.portamentoTime==0) {
				SweepLength = Duration;
				ManualSweep = true;
			} else {
				int squaredTime = Track.portamentoTime * Track.portamentoTime;
				SweepLength = (uint)(Math.Abs(SweepPitch) * squaredTime);
			}
		}
		
		internal void Update() {

			bool bNotInSustain = state != ChannelState.Sustain;
			bool bInStart = state == ChannelState.Start;
			bool bPitchSweep = SweepPitch!=0 && SweepLength!=0 && SweepCounter <= SweepLength;
			bool bModulation = Track!=null && Track.ModulationDepth != 0;
			bool bVolNeedUpdate = updateFlags.HasFlag(ChannelUpdateFlags.Volume) || bNotInSustain;
			bool bPanNeedUpdate = updateFlags.HasFlag(ChannelUpdateFlags.Pan) || bInStart;
			bool bTmrNeedUpdate = updateFlags.HasFlag(ChannelUpdateFlags.Timer) || bInStart || bPitchSweep;

			int modParam=0;

			switch(state) {
				case ChannelState.None:
					return;
				case ChannelState.Start:
					EnvelopeLevel = EnvelopeStartLevel;
					state = ChannelState.Attack;
					goto case ChannelState.Attack;
				case ChannelState.Attack:
					break;
				case ChannelState.Decay:
					EnvelopeLevel -= DecayRate;
					break;
				case ChannelState.Release:
					EnvelopeLevel -= ReleaseRate;
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

				modParam = Remap.Sine(ModulationCounter >> 8) * Track.ModulationRange * Track.ModulationDepth;
				if(Track.ModulationType==TrackPlayer.ModulationTypeEnum.Pitch) {
					modParam = modParam * 60 >> 14;
				} else {
					//modParam = (modParam & ~0xFC000000) >> 8) | ((((modParam < 0 ? -1 : 0) << 6) | (modParam >> 26)) << 18;
				}

				//TODO clean up the reverse engineered math and implement this
			}

			if(bTmrNeedUpdate) {
				int totaAdj = this.Note - Instrument.BaseNote * 64;
				totaAdj += Track.PitchBend * Track.PitchBendRange >> 1;

				if(bModulation && Track.ModulationType== TrackPlayer.ModulationTypeEnum.Pitch) {
					totaAdj += modParam;
				}

				if(bPitchSweep) {
					totaAdj += (int)(SweepPitch * (double)(SweepLength - SweepCounter) / SweepLength);
					if(!ManualSweep) {
						SweepCounter++;
					}
				}


			}

			if(bVolNeedUpdate ||bPanNeedUpdate) {
				if(bVolNeedUpdate) {
					int totalVol = this.EnvelopeLevel >> 7;
					totalVol += Track.sequencePlayer.MasterVolume;
					totalVol += Track.sequencePlayer.seqInfo.vol;
					totalVol += Remap.Level(Track.Volume);
					totalVol += Remap.Level(Track.Expression);
					totalVol += (int)Velocity;
					if(bModulation && Track.ModulationType==TrackPlayer.ModulationTypeEnum.Volume) {
						totalVol += modParam;
					}
				}

				if(bPanNeedUpdate) {
					int realPan = Instrument.Pan - 64;
					realPan += Track.Pan;
					if(bModulation && Track.ModulationType==TrackPlayer.ModulationTypeEnum.Pan) {
						realPan += modParam;
					}
					realPan += 64;
				}
			}
		}


		private void SetupMixerChannelForNote() {
			var pcmInstrument = instrument as PCMInstrument;
			if(pcmInstrument!=null) {
				mixerChannel.Mode = MixerChannel.MixerChannelMode.Pcm;
				var swar = Track.sequencePlayer.swars[pcmInstrument.swar];
				var swav = swar.waves[pcmInstrument.swav];
				return;
			}
			var pulseInstrument = instrument as PulseInstrument;
			if(pulseInstrument!=null) {
				mixerChannel.Mode = MixerChannel.MixerChannelMode.Pulse;
				mixerChannel.pulseWidth = pulseInstrument.Duty;
				return;
			}
			var noiseInstrument = instrument as NoiseInstrument;
			if(noiseInstrument!=null) {
				mixerChannel.Mode = MixerChannel.MixerChannelMode.Noise;
				return;
			}
			if(instrument == null) return;
			throw new InvalidOperationException();
		}

		[Flags]
		private enum ChannelUpdateFlags {
			Volume,
			Pan,
			Timer
		}

	}
}
