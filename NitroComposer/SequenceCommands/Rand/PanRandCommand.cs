namespace Henke37.Nitro.Composer.SequenceCommands.Rand {
	public class PanRandCommand : BaseSequenceCommand {
		public short PanMin, PanMax;

		public PanRandCommand(short min, short max) {
			this.PanMin = min;
			this.PanMax = max;
		}
	}
}
