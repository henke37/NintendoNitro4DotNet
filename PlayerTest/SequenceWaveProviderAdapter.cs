using NAudio.Wave;
using Henke37.Nitro.Composer.Player;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerTest {
	class SequenceWaveProviderAdapter : IWaveProvider {
		private BasePlayer player;
		private SamplePair[] sampleBuffer;

		private const int chanC = 2;

		public WaveFormat WaveFormat => new WaveFormat(player.SampleRate, 16, chanC);

		public SequenceWaveProviderAdapter(BasePlayer player) {
			this.player = player;
		}

		public SequenceWaveProviderAdapter(BasePlayer player, int sampleRate) {
			this.player = player;
			player.SampleRate = sampleRate;
		}

		public int Read(byte[] buffer, int offset, int count) {
			int samplesInBuff = count/(2*chanC);

			if(sampleBuffer == null) {
				sampleBuffer = new SamplePair[samplesInBuff * chanC];
			} else if(sampleBuffer.Length != samplesInBuff * chanC) {
				Array.Resize(ref sampleBuffer, samplesInBuff * chanC);
			}

			int samplesWritten=player.GenerateSamples(sampleBuffer);

			using(var w = new BinaryWriter(new MemoryStream(buffer, offset, samplesWritten * 4, true))) {
				for(int sampleIndex = 0; sampleIndex < samplesWritten; sampleIndex++) {
					w.Write((short)sampleBuffer[sampleIndex].Left);
					w.Write((short)sampleBuffer[sampleIndex].Right);
				}
			}

			return samplesWritten*2;
		}
	}
}
