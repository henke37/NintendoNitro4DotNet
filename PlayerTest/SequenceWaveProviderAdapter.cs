using NAudio.Wave;
using NitroComposerPlayer;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerTest {
	class SequenceWaveProviderAdapter : IWaveProvider {
		private SequencePlayer player;
		private int[] sampleBuffer;

		private const int chanC = 2;

		public WaveFormat WaveFormat => new WaveFormat(player.SampleRate, 16, chanC);

		public SequenceWaveProviderAdapter(SequencePlayer player, int sampleRate = 44100) {
			this.player = player;
			player.SampleRate = sampleRate;
		}

		public int Read(byte[] buffer, int offset, int count) {
			int samplesInBuff = count / (2 * chanC);

			if(sampleBuffer == null) {
				sampleBuffer = new int[samplesInBuff * chanC];
			} else if(sampleBuffer.Length != samplesInBuff * chanC) {
				Array.Resize(ref sampleBuffer, samplesInBuff * chanC);
			}
			player.GenerateSamples(sampleBuffer);


			using(var w = new BinaryWriter(new MemoryStream(buffer, offset, count, true))) {
				for(int sampleIndex = 0; sampleIndex < samplesInBuff; ++sampleIndex) {
					w.Write(sampleBuffer[sampleIndex]);
				}
			}

			return count;
		}
	}
}
