using Henke37.Nitro.Composer.SequenceCommands;
using System.Collections.Generic;

namespace Henke37.Nitro.Composer {
    public class Sequence {
        public List<BaseSequenceCommand> commands;

        public Sequence() {
            commands = new List<BaseSequenceCommand>();
        }
    }
}