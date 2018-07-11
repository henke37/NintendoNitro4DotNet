namespace Nitro.Composer.SequenceCommands {
    public class ReturnCommand : BaseSequenceCommand {
        public ReturnCommand() { }

        internal override bool EndsFlow => true;
    }
}
