namespace Nitro.Composer.SequenceCommands.Rand {
	public class PortamentoTimeRandCommand : BaseSequenceCommand {
		public uint TimeMin, TimeMax;
		public PortamentoTimeRandCommand(uint min, uint max) {
			TimeMin = min;
			TimeMax = max;
		}
	}
}
