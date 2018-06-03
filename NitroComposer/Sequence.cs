using NitroComposer.SequenceCommands;
using System.Collections.Generic;

namespace NitroComposer {
    public class Sequence {
        public List<BaseSequenceCommand> commands;

        public Sequence() {
            commands = new List<BaseSequenceCommand>();
        }
    }
}