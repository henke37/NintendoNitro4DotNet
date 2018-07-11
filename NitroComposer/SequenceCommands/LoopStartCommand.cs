namespace Nitro.Composer.SequenceCommands {
    public class LoopStartCommand : BaseSequenceCommand {
        public byte LoopCount;
        public LoopStartCommand(byte count) {
            LoopCount = count;
        }
    }
}
