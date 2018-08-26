using Nitro.Composer;
using Nitro.Composer.Instruments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroComposerSeqPlayer {
	public class SequencePlayer {
		private const int ChannelCount = 16;

		internal SBNK bank;
		internal SWAR[] swars;
		internal SSEQ sseq;
		internal SDat.PlayerInfoRecord player;

		internal TrackPlayer mainTrack;
		internal TrackPlayer[] tracks;
		internal ushort tempo = 120;

		internal Mixer mixer = new Mixer();

		public short[] Variables = new short[16] {
			-1, -1, -1, -1,
			-1, -1, -1, -1,
			-1, -1, -1, -1,
			-1, -1, -1, -1
		};

		private ChannelInfo[] channels;

		public SequencePlayer(SDat sdat, string sequenceName) {
			int seqIndex = sdat.seqSymbols.IndexOf(sequenceName);
			if(seqIndex == -1) throw new FileNotFoundException();
			Load(sdat,seqIndex);
		}
		public SequencePlayer(SDat sdat, int seqIndex) {
			Load(sdat, seqIndex);
			mainTrack = new TrackPlayer(this);
		}

		private void Load(SDat sdat, int seqIndex) {
			channels = new ChannelInfo[ChannelCount];
			for(int channelId=0;channelId< ChannelCount;++channelId) {
				channels[channelId] = new ChannelInfo(mixer.channels[channelId]);
			}

			var info = sdat.sequenceInfo[seqIndex];
			sseq = sdat.OpenSequence(seqIndex);
			player = sdat.playerInfo[info.player];
			LoadBank(sdat, info.bankId);
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

		private void RunCommandsForTracks() {
			foreach(var track in tracks) {
				while(track.ExecuteNextCommand()) ;
			}
		}

		private readonly int[] PCMChannelSearchList = new int[] { 4, 5, 6, 7, 2, 0, 3, 1, 8, 9, 10, 11, 14, 12, 15, 13 };
		private readonly int[] PulseChannelSearchList = new int[] { 8, 9, 10, 11, 12, 13 };
		private readonly int[] NoiseChannelSearchList = new int[] { 14, 15 };

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
					if(bestChannel.Prio > candidateChannel.Prio) continue;
					if(bestChannel.Vol > candidateChannel.Vol) continue;
				}

				bestChannel = candidateChannel;
			}

			return bestChannel;
		}
	}
}
