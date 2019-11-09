using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Nitro.Composer.Player {
	public abstract class BasePlayer {
		abstract public void GenerateSamples(SamplePair[] samples);

		public abstract int SampleRate { get; set; }
	}
}
