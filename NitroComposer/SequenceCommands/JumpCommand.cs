namespace NitroComposer.SequenceCommands {
    public class JumpCommand : BaseSequenceCommand {

        public uint target;
        public JumpType type;

        public JumpCommand(uint target, JumpType type) {
            this.target = target;
            this.type = type;
        }

        public enum JumpType {
            JUMP,
            CALL,
            OPEN_TRACK
        }

        internal override bool EndsFlow => type==JumpType.JUMP;
    }
}
