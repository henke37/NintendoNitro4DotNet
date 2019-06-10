namespace Henke37.Nitro.Composer.SequenceCommands {
    public class ExpressionCommand : BaseSequenceCommand {
        public byte Value;
        public ExpressionCommand(byte val) {
            Value = val;
        }
    }
}
