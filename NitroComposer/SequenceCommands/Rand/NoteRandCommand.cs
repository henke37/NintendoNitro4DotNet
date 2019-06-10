namespace Henke37.Nitro.Composer.SequenceCommands.Rand {
	public class NoteRandCommand : BaseSequenceCommand {
		public byte Note;
		public uint Velocity;
		public short DurationMin, DurationMax;

		public NoteRandCommand(byte note, uint vel, short durationMin, short durationMax) {
			Note = note;
			Velocity = vel;
			DurationMin = durationMin;
			DurationMax = durationMax;
		}
	}
}
