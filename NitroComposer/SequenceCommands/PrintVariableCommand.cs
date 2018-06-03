namespace NitroComposer.SequenceCommands {
    public class PrintVariableCommand : BaseSequenceCommand {
        public byte Variable;
        public PrintVariableCommand(byte var) {
            Variable = var;
        }
    }
}
