namespace Nitro.Composer.SequenceCommands.Rand {
	public class LoopStartRandCommand : BaseSequenceCommand {
		public short LoopCountMin, LoopCountMax;
		public LoopStartRandCommand(short min, short max) {
			LoopCountMin = min;
			LoopCountMax = max;
		}
	}
}
