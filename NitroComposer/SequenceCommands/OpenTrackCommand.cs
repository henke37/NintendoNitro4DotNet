namespace Nitro.Composer.SequenceCommands {
    public class OpenTrackCommand : JumpCommand {
        public byte Track;

        public OpenTrackCommand(byte track, uint target) : base(target, JumpType.OPEN_TRACK) {
            Track = track;
        }
    }
}
