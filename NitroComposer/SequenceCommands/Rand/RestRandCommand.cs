namespace Nitro.Composer.SequenceCommands.Rand {
	public class RestRandCommand : BaseSequenceCommand {
		public uint RestMin, RestMax;
		public RestRandCommand(uint min, uint max) {
			RestMin = min;
			RestMax = max;
		}
	}
}
