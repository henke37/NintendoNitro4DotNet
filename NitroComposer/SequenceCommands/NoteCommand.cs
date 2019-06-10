namespace Henke37.Nitro.Composer.SequenceCommands {
    public class NoteCommand : BaseSequenceCommand {
        public byte Note;
        public uint Velocity;
        public uint Duration;

        public NoteCommand(byte note, uint vel, uint duration) {
            Note = note;
            Velocity = vel;
            Duration = duration;
        }

        public bool InfiniteDuration {
            get => Duration == 0;
        }
    }
}
