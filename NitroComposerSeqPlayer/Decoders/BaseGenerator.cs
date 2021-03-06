﻿using System;
using System.Collections.Generic;

namespace Henke37.Nitro.Composer.Player.Decoders {
	internal abstract class BaseGenerator {

		protected double samplePosition=0;
		public double sampleIncrease=0;

		internal abstract int GetSample();

		public event Action SoundComplete;

		public virtual void IncrementSample() {
			samplePosition += sampleIncrease;
		}

		protected void OnSoundComplete() => SoundComplete?.Invoke();
	}
}
