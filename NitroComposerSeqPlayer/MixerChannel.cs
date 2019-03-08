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
				double timeConstant = SequencePlayer.ARM7_CLOCK / ((double)(SampleRate * 2));
				Generator.sampleIncrease = timeConstant / (0x10000 - value);
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
		public double Loudness { get => (VolMul/128.0) / (1<<VolShift) ; }

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

		public void GenerateSample(out int leftChan, out int rightChan) {
			if(Mode == MixerChannelMode.Off) {
				leftChan = 0;
				rightChan = 0;
				return;
			}

			int sample = Generator.GetSample();
			Generator.IncrementSample();

			sample = Remap.MulDiv7(sample, VolMul) >> VolShift;
			leftChan = Remap.MulDiv7(sample, 127 - Pan);
			rightChan = Remap.MulDiv7(sample, Pan);
		}

		internal void SetSampleData(Wave wave) {
			Mode = MixerChannelMode.Pcm;
			BaseSampleDecoder decoder = BaseSampleDecoder.CreateDecoder(wave.Encoding);
			decoder.Init(new BinaryReader(wave.dataStream));
			decoder.Loops = wave.Loops;
			decoder.LoopLength = wave.LoopLength;
			decoder.TotalLength = wave.LoopStart + wave.LoopLength;
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

		internal int SampleRate;

		private string InstrumentString() {
			switch(Generator) {
				case ADPCMDecoder adpcm:
					return "ADPCM";
				case PCM8Decoder pcm8:
					return "PCM8";
				case PCM16Decoder pcm16:
					return "PCM16";
				case BaseSampleDecoder dec:
					return "PCMlike";
				case PulseGenerator pulse:
					return "Pulse " + pulse.ToString();
				case NoiseGenerator noise:
					return "Noise";
				case null:
					return "Off";
				default:
					return base.ToString();
			}
		}

		public override string ToString() {
			if(Mode == MixerChannelMode.Off) return "Off";
			return $"{InstrumentString()} {Loudness*100:f2}%";
		}
	}
}