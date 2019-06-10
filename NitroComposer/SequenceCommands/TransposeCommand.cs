namespace Henke37.Nitro.Composer.SequenceCommands {
    public class TransposeCommand : BaseSequenceCommand {
        public byte Transpose;

        public TransposeCommand(byte transpose) {
            Transpose = transpose;
        }
    }
}
