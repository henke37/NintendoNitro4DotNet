namespace Nitro.Composer.SequenceCommands.Rand {
	public class PitchBendRandCommand : BaseSequenceCommand {
		public uint BendMin, BendMax;
		public bool IsRange;

		public PitchBendRandCommand(uint bendMin, uint bendMax, bool range) {
			BendMin = bendMin;
			BendMax = bendMax;
			IsRange = range;
		}
	}
}
