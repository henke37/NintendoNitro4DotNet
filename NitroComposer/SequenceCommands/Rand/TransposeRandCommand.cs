namespace Nitro.Composer.SequenceCommands.Rand {
	public class TransposeRandCommand : BaseSequenceCommand {
		public short TransposeMin, TransposeMax;

		public TransposeRandCommand(short min, short max) {
			TransposeMin = min;
			TransposeMax = max;
		}
	}
}
