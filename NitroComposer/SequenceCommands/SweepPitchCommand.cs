using System;

namespace Henke37.Nitro.Composer.SequenceCommands {
    public class SweepPitchCommand : BaseSequenceCommand {
        public UInt16 Ammount;
        public SweepPitchCommand(UInt16 ammount) {
            Ammount = ammount;
        }
    }
}
