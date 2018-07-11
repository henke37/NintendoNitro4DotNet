namespace Nitro.Composer.SequenceCommands {
    public class PortamentoCommand : BaseSequenceCommand {
        public bool Enable;

        public PortamentoCommand(bool enable) {
            Enable = enable;
        }
    }
}
