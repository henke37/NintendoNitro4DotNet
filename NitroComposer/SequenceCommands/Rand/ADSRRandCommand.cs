namespace Nitro.Composer.SequenceCommands.Rand {
	public class ADSRRandCommand : BaseSequenceCommand {
		public short Min, Max;
		public ADSRCommand.EnvPos envPos;

		public ADSRRandCommand(ADSRCommand.EnvPos envPos, short min, short max) {
			this.envPos = envPos;
			Min = min;
			Max = max;
		}
	}
}
