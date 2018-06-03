namespace NitroComposer.SequenceCommands {
    public class VolumeCommand : BaseSequenceCommand {
        public bool Master;
        public byte Volume;

        public VolumeCommand(byte vol, bool master) {
            Volume = vol;
            Master = master;
        }
    }
}
