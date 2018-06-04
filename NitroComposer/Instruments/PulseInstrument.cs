using HenkesUtils;
using System;
using System.IO;

namespace NitroComposer.Instruments {
    public class PulseInstrument : BaseLeafInstrument {
        public UInt16 Duty;

        public PulseInstrument(BinaryReader r) {
            Duty = r.ReadUInt16();
            if(Duty >= 8) throw new InvalidDataException("Duty has to be less than 8");
            r.Skip(2);
            parseFields(r);
        }
    }
}
