using System;

namespace Nitro.Composer.SequenceCommands {
    public class SweepPitchCommand : BaseSequenceCommand {
        UInt16 Ammount;
        public SweepPitchCommand(UInt16 ammount) {
            Ammount = ammount;
        }
    }
}
