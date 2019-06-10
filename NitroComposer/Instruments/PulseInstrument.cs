using Henke37.IOUtils;
using System;
using System.IO;

namespace Henke37.Nitro.Composer.Instruments {
    public class PulseInstrument : BaseLeafInstrument {
        public UInt16 Duty;

        public PulseInstrument(BinaryReader r) {
            Duty = r.ReadUInt16();
            if(Duty >= 8) throw new InvalidDataException("Duty has to be less than 8");
            r.Skip(2);
            parseFields(r);
        }

		public override string ToString() {
			return string.Format("{1} {0}",dutyMap[Duty],base.ToString());
		}

		private static string[] dutyMap = new string[] {
			"12.5%", "25.0%", "37.5%", "50.0%",
			"62.5%", "75.0%", "87.5%", "0.0%"
		};
	}
}
