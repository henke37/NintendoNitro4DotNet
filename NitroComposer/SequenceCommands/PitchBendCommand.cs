namespace Henke37.Nitro.Composer.SequenceCommands {
    public class PitchBendCommand : BaseSequenceCommand {
        public byte Bend;
        public bool IsRange;

        public PitchBendCommand(byte bend, bool range) {
            Bend = bend;
            IsRange = range;
        }
    }
}
