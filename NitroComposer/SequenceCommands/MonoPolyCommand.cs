namespace NitroComposer.SequenceCommands {
    public class MonoPolyCommand : BaseSequenceCommand {
        public bool IsMono;

        public MonoPolyCommand(bool mono) {
            IsMono = mono;
        }
    }
}
