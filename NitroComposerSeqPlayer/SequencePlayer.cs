using Henke37.Nitro.Composer;
using Henke37.Nitro.Composer.Instruments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Nitro.Composer.Player {
	public class SequencePlayer : BasePlayer {
		private const int ChannelCount = 16;

		internal SBNK bank;
		internal SWAR[] swars;
		internal SSEQ sseq;
		internal SDat.PlayerInfoRecord player;
		internal SDat.SequenceInfoRecord seqInfo;

		private const int TRACKPLAYER_COUNT=16;
		internal TrackPlayer mainTrack;
		internal TrackPlayer[] tracks;

		private short tempoCounter = 0;
		private const short TempoBase = 240;
		internal ushort Tempo = 120;
		public ushort TempoRate = 0x100;

		internal byte MasterVolume;

		private readonly Mixer mixer = new Mixer();

		public override int SampleRate {
			get => _sampleRate;
			set {
				mixer.SampleRate = value;
				_sampleRate = value;
			}
		}
		private int _sampleRate;

		public short[] Variables = new short[16] {
			-1, -1, -1, -1,
			-1, -1, -1, -1,
			-1, -1, -1, -1,
			-1, -1, -1, -1
		};

		internal ChannelInfo[] channels;
		private float secondsIntoPlayback=0;
		private float secondsUntilNextClock=0;

		public SequencePlayer(SDat sdat, string sequenceName) {
			int seqIndex = sdat.ResolveSeqName(sequenceName);
			if(seqIndex == -1) throw new FileNotFoundException();
			Load(sdat,seqIndex);
		}
		public SequencePlayer(SDat sdat, int seqIndex) {
			Load(sdat, seqIndex);
		}

		private void Load(SDat sdat, int seqIndex) {
			channels = new ChannelInfo[ChannelCount];
			for(int channelId=0;channelId< ChannelCount;++channelId) {
				channels[channelId] = new ChannelInfo(mixer.channels[channelId]);
			}

			seqInfo = sdat.sequenceInfo[seqIndex];
			player = sdat.playerInfo[seqInfo.player];

			sseq = sdat.OpenSequence(seqIndex);
			LoadBank(sdat, seqInfo.bankId);

			tracks = new TrackPlayer[TRACKPLAYER_COUNT];
			tracks[0]=mainTrack = new TrackPlayer(this);
		}

		private void LoadBank(SDat sdat, ushort bankId) {
			var info = sdat.bankInfo[bankId];
			bank = sdat.OpenBank(bankId);
			swars = new SWAR[4];
			for(int i=0;i<4;++i) {
				if(info.swars[i] == -1) continue;
				swars[i] = sdat.OpenWaveArchive(info.swars[i]);
			}
		}

		private void Update() {
			for(; tempoCounter >= 0; tempoCounter -= TempoBase) {
				foreach(var track in tracks) {
					if(track == null) continue;
					while(track.ExecuteNextCommand()) ;
				}
			}
			
			foreach(var chan in channels) {
				chan.UpdateTrackData();
			}
			foreach(var track in tracks) {
				if(track == null) continue;
				track.updateFlags = 0;
			}
			foreach(var chan in channels) {
				chan.Update();
			}
			
			tempoCounter += (short)(Tempo * TempoRate >> 8);
		}

		private readonly int[] PCMChannelSearchList = new int[] { 4, 5, 6, 7, 2, 0, 3, 1, 8, 9, 10, 11, 14, 12, 15, 13 };
		private readonly int[] PulseChannelSearchList = new int[] { 8, 9, 10, 11, 12, 13 };
		private readonly int[] NoiseChannelSearchList = new int[] { 14, 15 };
		private float secondsPerSample { get => 1 / ((float)SampleRate); }

		internal ChannelInfo FindChannelForInstrument(Instrument instrument) {
			int[] channelSearchList;

			if(instrument is PCMInstrument) {
				channelSearchList = PCMChannelSearchList;
			} else if(instrument is PulseInstrument) {
				channelSearchList = PulseChannelSearchList;
			} else {
				channelSearchList = NoiseChannelSearchList;
			}

			ChannelInfo bestChannel = null;

			foreach(int candidateChannelID in channelSearchList) {
				if(player.channelMask != 0) {
					bool allowed = (player.channelMask & (1 << candidateChannelID)) != 0;
					if(!allowed) continue;
				}
				var candidateChannel = channels[candidateChannelID];

				if(bestChannel!=null) {
					if(bestChannel.Prio >= candidateChannel.Prio) continue;
					if(bestChannel.Vol >= candidateChannel.Vol) continue;
				}

				bestChannel = candidateChannel;
			}

			return bestChannel;
		}


		public override void GenerateSamples(SamplePair[] samples) {
			for(int sampleIndex=0;sampleIndex<samples.Length;sampleIndex+=2) {
				samples[sampleIndex]=mixer.GenerateSamplePair();

				secondsIntoPlayback += secondsPerSample;

				if(secondsIntoPlayback > secondsUntilNextClock) {
					Update();
					secondsUntilNextClock += SecondsPerClockCycle;
				}
			}
		}

		internal const int ARM7_CLOCK = 33513982;
		private const float SecondsPerClockCycle = 64 * 2728 / (float)ARM7_CLOCK;
	}
}
