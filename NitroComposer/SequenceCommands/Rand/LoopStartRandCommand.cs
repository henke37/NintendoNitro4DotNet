namespace Nitro.Composer.SequenceCommands.Rand {
	public class LoopStartRandCommand : BaseSequenceCommand {
		public int LoopCountMin, LoopCountMax;
		public LoopStartRandCommand(int min, int max) {
			LoopCountMin = min;
			LoopCountMax = max;
		}
	}
}
