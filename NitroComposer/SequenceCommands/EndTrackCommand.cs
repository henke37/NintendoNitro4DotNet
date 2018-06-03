namespace NitroComposer.SequenceCommands {
    public class EndTrackCommand : BaseSequenceCommand {
        public EndTrackCommand() { }
        internal override bool EndsFlow => true;
    }
}
