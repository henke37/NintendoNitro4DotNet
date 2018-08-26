namespace Nitro.Composer.SequenceCommands.Rand {
	public class ExpressionRandCommand : BaseSequenceCommand {
		public short Min, Max;
		public ExpressionRandCommand(short min, short max) {
			Min = min;
			Max = max;
		}
	}
}
