using System;
using System.IO;

namespace Nitro.Composer.Instruments {
    public class PCMInstrument : BaseLeafInstrument {

        public UInt16 swar;
        public UInt16 swav;

        public PCMInstrument(BinaryReader r) {
            swar = r.ReadUInt16();
            swav = r.ReadUInt16();
            parseFields(r);
        }

		public override string ToString() {
			return string.Format("{2} {0} {1}", swar, swav,base.ToString());
		}

	}
}