namespace Nitro.Composer.SequenceCommands.Rand {
	public class ProgramChangeRandCommand : BaseSequenceCommand {
		public short ProgramMin, ProgramMax;

		public ProgramChangeRandCommand(short min, short max) {
			this.ProgramMin = min;
			this.ProgramMax = max;
		}
	}
}
