using System;

namespace Nitro.Composer.SequenceCommands.Rand {
	public class SweepPitchRandCommand : BaseSequenceCommand {
		public short AmmountMin, AmmountMax;
		public SweepPitchRandCommand(short min, short max) {
			AmmountMin = min;
			AmmountMax = max;
		}
	}
}
