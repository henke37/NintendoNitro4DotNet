namespace Henke37.Nitro.Composer.SequenceCommands {
    public class ADSRCommand : BaseSequenceCommand {
        public byte Value;
        public EnvPos envPos;

        public ADSRCommand(EnvPos envPos, byte val) {
            this.envPos = envPos;
            Value = val;
        }

        public enum EnvPos : byte {
            ATTACK=0xD0,
            DECAY,
            SUSTAIN,
            RELEASE
        }
    }
}
