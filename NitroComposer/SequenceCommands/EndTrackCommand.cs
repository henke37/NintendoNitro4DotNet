namespace Henke37.Nitro.Composer.SequenceCommands {
    public class EndTrackCommand : BaseSequenceCommand {
        public EndTrackCommand() { }
        internal override bool EndsFlow => true;
    }
}
