namespace NitroComposer.SequenceCommands {
    public class ProgramChangeCommand : BaseSequenceCommand {
        public uint Program;

        public ProgramChangeCommand(uint Program) {
            this.Program = Program;
        }
    }
}
