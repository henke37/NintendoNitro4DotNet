namespace Henke37.Nitro.Composer.SequenceCommands.Rand {
	public class PitchBendRandCommand : BaseSequenceCommand {
		public short BendMin, BendMax;
		public bool IsRange;

		public PitchBendRandCommand(short bendMin, short bendMax, bool range) {
			BendMin = bendMin;
			BendMax = bendMax;
			IsRange = range;
		}
	}
}
