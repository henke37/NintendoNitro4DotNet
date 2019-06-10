namespace Henke37.Nitro.Composer.SequenceCommands {
    public class PortamentoTimeCommand : BaseSequenceCommand {
        public byte Time;
        public PortamentoTimeCommand(byte time) {
            Time = time;
        }
    }
}
