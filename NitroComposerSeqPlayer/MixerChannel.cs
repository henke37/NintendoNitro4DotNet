using System;
using System.IO;
using Nitro.Composer;
using NitroComposerPlayer.Decoders;

namespace NitroComposerPlayer {
	internal class MixerChannel {
		private ushort _timer;

		public ushort Timer {
			get => _timer;
			set {
				_timer = value;

				Generator.sampleIncrease = (uint)(ARM7_CLOCK / (sampleRate * 2) / (0x10000 - value));
			}
		}

		private BaseGenerator Generator {
			get => _generator;
			set {
				if(_generator == value) return;
				if(_generator != null) _generator.SoundComplete -= Generator_OnSoundComplete;
				_generator = value;
				if(value != null) value.SoundComplete += Generator_OnSoundComplete;
			}
		}

		private BaseSampleDecoder Decoder { get => (BaseSampleDecoder)_generator; }

		internal uint TotalLength { set => Decoder.TotalLength = value; get => Decoder.TotalLength; }
		internal uint LoopLength { set => Decoder.LoopLength = value; get => Decoder.LoopLength; }
		internal bool Loops { set => Decoder.Loops = value; get => Decoder.Loops; }

		internal MixerChannelMode Mode { get; private set; } = MixerChannelMode.Off;

		private void Generator_OnSoundComplete() {
			OnSoundComplete?.Invoke();
		}

		private BaseGenerator _generator;

		internal int Pan;
		internal int VolMul;
		internal int VolShift;

		public event Action OnSoundComplete;

		public enum MixerChannelMode {
			Off,
			Pcm,
			Pulse,
			Noise
		}

		public void Reset() {
			Mode = MixerChannelMode.Off;
			Generator = null;
		}

		public int GenerateBaseSample() {
			if(Mode == MixerChannelMode.Off) return 0;
			return Generator.GetSample();
		}

		public void GenerateSample(out int leftChan, out int rightChan) {
			int sample = GenerateBaseSample();
			sample = Remap.MulDiv7(sample, VolMul) >> VolShift;
			leftChan = Remap.MulDiv7(sample, 127 - Pan);
			rightChan = Remap.MulDiv7(sample, Pan);
		}

		internal void SetSampleData(Stream dataStream, WaveEncoding encoding) {
			Mode = MixerChannel.MixerChannelMode.Pcm;
			BaseSampleDecoder decoder = BaseSampleDecoder.CreateDecoder(encoding);
			decoder.Init(new BinaryReader(dataStream));
			Generator = decoder;
		}

		internal void SetPulse(ushort duty) {
			Mode = MixerChannelMode.Pulse;
			Generator = new PulseGenerator(duty);
		}

		internal void SetNoise() {
			Mode = MixerChannelMode.Noise;
			Generator = new NoiseGenerator();
		}

		private readonly int sampleRate;
		private const int ARM7_CLOCK = 33513982;

		
	}
}