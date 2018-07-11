namespace Nitro.Composer.SequenceCommands.Rand {
	public class PanRandCommand : BaseSequenceCommand {
		public uint PanMin, PanMax;

		public PanRandCommand(uint min, uint max) {
			this.PanMin = min;
			this.PanMax = max;
		}
	}
}
