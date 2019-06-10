namespace Henke37.Nitro.Composer.SequenceCommands {
    public class PortamentoKeyCommand : BaseSequenceCommand {
        public byte Key;
        public PortamentoKeyCommand(byte key) {
            Key = key;
        }
    }
}
