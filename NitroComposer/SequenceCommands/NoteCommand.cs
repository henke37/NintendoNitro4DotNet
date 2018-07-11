namespace Nitro.Composer.SequenceCommands {
    public class NoteCommand : BaseSequenceCommand {
        public uint Note;
        public uint Velocity;
        public uint Duration;

        public NoteCommand(uint note, uint vel, uint duration) {
            Note = note;
            Velocity = vel;
            Duration = duration;
        }

        public bool InfiniteDuration {
            get => Duration == 0;
        }
    }
}
