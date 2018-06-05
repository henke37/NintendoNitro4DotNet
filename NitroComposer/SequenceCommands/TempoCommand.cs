using System;

namespace NitroComposer.SequenceCommands {
    public class TempoCommand : BaseSequenceCommand {
        public UInt16 Tempo;
        public TempoCommand(UInt16 tempo) {
            Tempo = tempo;
        }
    }
}
