using Nitro.Composer;
using System;
using System.Collections.Generic;
using System.IO;

namespace NitroComposerPlayer {
	/* Stream player included in the same assembly for convenience.
	 * You will most likely want support for both. */
	public class StreamPlayer {

		private STRM strm;

		public StreamPlayer(SDat sdat, string streamName) {
			int streamIndex = sdat.streamSymbols.IndexOf(streamName);
			if(streamIndex == -1) throw new FileNotFoundException();
			Load(sdat, streamIndex);
		}

		public StreamPlayer(SDat sdat, int streamIndex) {
			Load(sdat, streamIndex);
		}

		private void Load(SDat sdat, int streamIndex) {
			strm=sdat.OpenStream(streamIndex);
		}
	}
}
