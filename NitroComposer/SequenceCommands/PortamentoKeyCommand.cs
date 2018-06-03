namespace NitroComposer.SequenceCommands {
    public class PortamentoKeyCommand : BaseSequenceCommand {
        public byte Key;
        public PortamentoKeyCommand(byte key) {
            Key = key;
        }
    }
}
