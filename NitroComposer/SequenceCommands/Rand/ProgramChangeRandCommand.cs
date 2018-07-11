namespace Nitro.Composer.SequenceCommands.Rand {
	public class ProgramChangeRandCommand : BaseSequenceCommand {
		public uint ProgramMin, ProgramMax;

		public ProgramChangeRandCommand(uint min, uint max) {
			this.ProgramMin = min;
			this.ProgramMax = max;
		}
	}
}
