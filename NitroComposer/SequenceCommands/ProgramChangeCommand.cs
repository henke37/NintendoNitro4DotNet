namespace Henke37.Nitro.Composer.SequenceCommands {
    public class ProgramChangeCommand : BaseSequenceCommand {
        public uint Program;

        public ProgramChangeCommand(uint Program) {
            this.Program = Program;
        }
    }
}
