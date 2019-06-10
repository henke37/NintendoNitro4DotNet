using System;

namespace Henke37.Nitro.Composer.SequenceCommands {
    public class TempoCommand : BaseSequenceCommand {
        public UInt16 Tempo;
        public TempoCommand(UInt16 tempo) {
            Tempo = tempo;
        }
    }
}
