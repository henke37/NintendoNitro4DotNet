using System;
using System.Collections.Generic;

namespace NitroComposerPlayer.Decoders {
	internal abstract class BaseGenerator {

		protected uint samplePosition;
		public uint sampleIncrease;

		internal abstract int GetSample();

		public event Action SoundComplete;

		public virtual void IncrementSample() {
			samplePosition += sampleIncrease;
		}

		protected void OnSoundComplete() => SoundComplete?.Invoke();
	}
}
