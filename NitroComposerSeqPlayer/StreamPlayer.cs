using Henke37.IOUtils;
using Henke37.Nitro.Composer;
using Henke37.Nitro.Composer.Player.Decoders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Henke37.Nitro.Composer.Player {
	/* Stream player included in the same assembly for convenience.
	 * You will most likely want support for both. */
	public class StreamPlayer : BasePlayer {

		private STRM strm;

		private BaseSampleDecoder[] decoders;

		private int loadedBlock;
		private uint currentSamplePos;
		private uint samplesLeftInBlock;

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

			Reset();

			decoders = new BaseSampleDecoder[strm.channels];
			for(int channel=0;channel<strm.channels;++channel) {
				var decoder= BaseSampleDecoder.CreateDecoder(strm.encoding);
				decoder.sampleIncrease = 1;
				decoders[channel] = decoder;
			}
		}

		private void Reset() {
			loadedBlock = -1;
			currentSamplePos = 0;
		}

		public override int SampleRate { get; set; }

		public override int GenerateSamples(SamplePair[] samples) {
			if(strm.channels==2) {
				return GenerateStereoSamples(samples);
			} else {
				return GenerateMonoSamples(samples);
			}
		}

		private int GenerateMonoSamples(SamplePair[] samples) {
			throw new NotImplementedException();
		}

		private int GenerateStereoSamples(SamplePair[] samples) {
			int sampleIndex = 0;

			while(sampleIndex < samples.Length) {
				//ensure that the correct block is loaded
				uint targetSamplePos = currentSamplePos;

				if(samplesLeftInBlock <= 0 && loadedBlock == strm.nBlock-1) {
					if(strm.loop) {
						targetSamplePos = strm.loopPoint;
					} else {
						OnComplete();
						return sampleIndex;
					}
				}

				int targetBlock = (int)(targetSamplePos / strm.blockSamples);

				if(loadedBlock != targetBlock) {
					LoadBlock(targetBlock);
				}
				//fastforward the block if needed
				while(currentSamplePos < targetSamplePos) {
					StepSamplePos();
				}
				//decode the samples
				for(; sampleIndex < samples.Length; ++sampleIndex) {
					if(samplesLeftInBlock <= 0) break;

					samples[sampleIndex] = new SamplePair(
						decoders[0].GetSample(), 
						decoders[1].GetSample()
					);

					StepSamplePos();
				}
			}
			return sampleIndex;
		}

		private void StepSamplePos() {
			foreach(var decoder in decoders) {
				decoder.IncrementSample();
			}
			currentSamplePos++;
			samplesLeftInBlock--;
		}

		private void LoadBlock(int blockId) {
			bool lastBlock = blockId+1 >= strm.nBlock;
			samplesLeftInBlock = lastBlock ? strm.lastBlockSamples : strm.blockSamples;

			for(int channel=0;channel<strm.channels;++channel) {
				var decoder = decoders[channel];
				decoder.Init(
					new BinaryReader(GetBlockStream(blockId, channel)),
					samplesLeftInBlock,
					false
				);
			}
			loadedBlock = blockId;


			currentSamplePos = strm.blockSamples * (uint)blockId;
		}

		private Stream GetBlockStream(int blockId, int channel) {
			bool lastBlock = blockId + 1 >= strm.nBlock;
			long blockLen = lastBlock ? strm.lastBlockLength : strm.blockLength;
			long offset = strm.blockLength * strm.channels * blockId + blockLen * channel;

			return new SubStream(strm.dataStream, offset, blockLen);
		}
	}
}
