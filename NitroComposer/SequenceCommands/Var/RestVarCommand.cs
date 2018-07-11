using NitroComposer.SequenceCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Composer.SequenceCommands.Var {
	public class RestVarCommand : BaseSequenceCommand {
		public uint RestVar;
		public RestVarCommand(uint rest) {
			RestVar = rest;
		}
	}
}
