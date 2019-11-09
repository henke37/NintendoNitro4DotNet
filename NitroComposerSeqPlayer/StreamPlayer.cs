using Henke37.Nitro.Composer;
using Henke37.Nitro.Composer.Player.Decoders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Composer.Player {
	/* Stream player included in the same assembly for convenience.
	 * You will most likely want support for both. */
	public class StreamPlayer : BasePlayer {

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

		public override int SampleRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override void GenerateSamples(SamplePair[] samples) {
			if(strm.channels==2) {
				GenerateStereoSamples(samples);
			} else {
				GenerateMonoSamples(samples);
			}
		}

		private void GenerateMonoSamples(SamplePair[] samples) {
			throw new NotImplementedException();
		}

		private void GenerateStereoSamples(SamplePair[] samples) {
			
			//ensure that the correct chunk is loaded
			//fastforward the chunk if needed
			//decode the samples
			//resample the samples
		}
	}
}
