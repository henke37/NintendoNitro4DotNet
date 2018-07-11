namespace Nitro.Composer.SequenceCommands {
    public class PriorityCommand : BaseSequenceCommand {
        public byte Priority;

        public PriorityCommand(byte prio) {
            Priority = prio;
        }
    }
}
