namespace Nitro.Composer.SequenceCommands.Rand {
	public class ExpressionRandCommand : BaseSequenceCommand {
		public uint Min, Max;
		public ExpressionRandCommand(uint min, uint max) {
			Min = min;
			Max = max;
		}
	}
}
