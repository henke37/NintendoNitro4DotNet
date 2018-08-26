namespace Nitro.Composer.SequenceCommands.Rand {
	public class NoteRandCommand : BaseSequenceCommand {
		public uint Note;
		public uint Velocity;
		public short DurationMin, DurationMax;

		public NoteRandCommand(uint note, uint vel, short durationMin, short durationMax) {
			Note = note;
			Velocity = vel;
			DurationMin = durationMin;
			DurationMax = durationMax;
		}
	}
}
