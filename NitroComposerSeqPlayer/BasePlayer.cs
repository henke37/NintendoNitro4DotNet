using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Nitro.Composer.Player {
	public abstract class BasePlayer {

		public event Action Complete;

		abstract public int GenerateSamples(SamplePair[] samples);

		public abstract int SampleRate { get; set; }

		protected void OnComplete() { Complete?.Invoke(); }
	}
}
