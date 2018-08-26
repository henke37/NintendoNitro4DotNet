namespace Nitro.Composer.SequenceCommands.Rand {
	public class PortamentoTimeRandCommand : BaseSequenceCommand {
		public short TimeMin, TimeMax;
		public PortamentoTimeRandCommand(short min, short max) {
			TimeMin = min;
			TimeMax = max;
		}
	}
}
