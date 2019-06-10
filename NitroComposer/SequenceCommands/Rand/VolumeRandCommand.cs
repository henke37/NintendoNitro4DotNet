namespace Henke37.Nitro.Composer.SequenceCommands.Rand {
	public class VolumeRandCommand : BaseSequenceCommand {
		public bool Master;
		public short VolumeMin, VolumeMax;

		public VolumeRandCommand(short min, short max, bool master) {
			VolumeMin = min;
			VolumeMax = max;
			Master = master;
		}
	}
}
