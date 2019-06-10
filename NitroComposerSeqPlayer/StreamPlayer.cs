using Henke37.Nitro.Composer;
using NitroComposerPlayer.Decoders;
using System;
using System.Collections.Generic;
using System.IO;

namespace NitroComposerPlayer {
	/* Stream player included in the same assembly for convenience.
	 * You will most likely want support for both. */
	public class StreamPlayer {

		private STRM strm;

		private BaseSampleDecoder[] decoders;

		public StreamPlayer(SDat sdat, string streamName) {
			var strm = sdat.OpenStream(streamName);
			Load(strm);
		}

		public StreamPlayer(SDat sdat, int streamIndex) {
			Load(sdat, streamIndex);
		}

		public StreamPlayer(STRM strm) {
			Load(strm);
		}

		private void Load(SDat sdat, int streamIndex) {
			var strm=sdat.OpenStream(streamIndex);
			Load(strm);
		}

		private void Load(STRM strm) {
			this.strm = strm;

			decoders = new BaseSampleDecoder[strm.channels];
			for(int channel=0;channel<strm.channels;++channel) {
				decoders[channel] = BaseSampleDecoder.CreateDecoder(strm.encoding);
			}
		}
	}
}
