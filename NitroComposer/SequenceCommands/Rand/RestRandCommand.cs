namespace Nitro.Composer.SequenceCommands.Rand {
	public class RestRandCommand : BaseSequenceCommand {
		public short RestMin, RestMax;
		public RestRandCommand(short min, short max) {
			RestMin = min;
			RestMax = max;
		}
	}
}
