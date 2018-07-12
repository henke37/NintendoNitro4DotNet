using System;

namespace Nitro.Composer.SequenceCommands.Rand {
	public class SweepPitchRandCommand : BaseSequenceCommand {
		public UInt16 AmmountMin, AmmountMax;
		public SweepPitchRandCommand(UInt16 min, UInt16 max) {
			AmmountMin = min;
			AmmountMax = max;
		}
	}
}
