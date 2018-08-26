using Nitro.Composer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroComposerSeqPlayer {
	public class SequencePlayer {
		internal SBNK bank;
		internal SWAR[] swars;
		internal SSEQ sseq;

		internal TrackPlayer mainTrack;
		internal TrackPlayer[] tracks;
		internal ushort tempo=120;

		public short[] Variables = new short[16];

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
			var info = sdat.sequenceInfo[seqIndex];
			sseq = sdat.OpenSequence(seqIndex);
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
	}
}
