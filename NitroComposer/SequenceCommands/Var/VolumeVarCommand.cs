using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro.Composer.SequenceCommands.Var {
	public class VolumeVarCommand : BaseSequenceCommand {
		public bool Master;
		public byte VolumeVar;

		public VolumeVarCommand(byte volVar, bool master) {
			VolumeVar = volVar;
			Master = master;
		}
	}
}
