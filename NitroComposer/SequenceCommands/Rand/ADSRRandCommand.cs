namespace Nitro.Composer.SequenceCommands.Rand {
	public class ADSRRandCommand : BaseSequenceCommand {
		public uint Min, Max;
		public ADSRCommand.EnvPos envPos;

		public ADSRRandCommand(ADSRCommand.EnvPos envPos, uint min, uint max) {
			this.envPos = envPos;
			Min = min;
			Max = max;
		}
	}
}
