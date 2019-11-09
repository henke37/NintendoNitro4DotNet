using Henke37.IOUtils;
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
			//ensure that the correct block is loaded
			//LoadBlock();
			//fastforward the block if needed
			//decode the samples
			//resample the samples
		}

		private void LoadBlock(int chunkId) {
			for(int channel=0;channel<strm.channels;++channel) {
				var decoder = decoders[channel];
				decoder.Init(new BinaryReader(GetBlockStream(chunkId, channel)));
			}
		}

		private Stream GetBlockStream(int chunkId, int channel) {
			bool lastBlock = strm.nBlock < chunkId;
			long blockLen = lastBlock ? strm.lastBlockLength : strm.blockLength;
			long offset = strm.blockLength * strm.channels * chunkId + blockLen * channel;

			return new SubStream(strm.dataStream, offset, blockLen);
		}
	}
}
