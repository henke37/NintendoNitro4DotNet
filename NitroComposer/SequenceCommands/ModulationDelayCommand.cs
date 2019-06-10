using System;

namespace Henke37.Nitro.Composer.SequenceCommands {
    public class ModulationDelayCommand : BaseSequenceCommand {
        public UInt16 Delay;
        public ModulationDelayCommand(UInt16 delay) {
            Delay = delay;
        }
    }
}
