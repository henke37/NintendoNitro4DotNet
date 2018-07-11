namespace Nitro.Composer.SequenceCommands {
    public class MonoPolyCommand : BaseSequenceCommand {
        public bool IsMono;

        public MonoPolyCommand(bool mono) {
            IsMono = mono;
        }
    }
}
