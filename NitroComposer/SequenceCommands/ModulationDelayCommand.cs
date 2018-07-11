using System;

namespace Nitro.Composer.SequenceCommands {
    public class ModulationDelayCommand : BaseSequenceCommand {
        UInt16 Delay;
        public ModulationDelayCommand(UInt16 delay) {
            Delay = delay;
        }
    }
}
