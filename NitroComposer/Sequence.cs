using Nitro.Composer.SequenceCommands;
using System.Collections.Generic;

namespace Nitro.Composer {
    public class Sequence {
        public List<BaseSequenceCommand> commands;

        public Sequence() {
            commands = new List<BaseSequenceCommand>();
        }
    }
}