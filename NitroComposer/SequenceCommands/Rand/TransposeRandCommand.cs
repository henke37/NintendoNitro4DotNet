namespace Nitro.Composer.SequenceCommands.Rand {
	public class TransposeRandCommand : BaseSequenceCommand {
		public uint TransposeMin, TransposeMax;

		public TransposeRandCommand(uint min, uint max) {
			TransposeMin = min;
			TransposeMax = max;
		}
	}
}
