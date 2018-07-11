namespace Nitro.Composer.SequenceCommands.Rand {
	public class NoteRandCommand : BaseSequenceCommand {
		public uint Note;
		public uint Velocity;
		public uint DurationMin, DurationMax;

		public NoteRandCommand(uint note, uint vel, uint durationMin, uint durationMax) {
			Note = note;
			Velocity = vel;
			DurationMin = durationMin;
			DurationMax = durationMax;
		}
	}
}
