using System;
using System.IO;

namespace NitroComposer.Instruments {
    public class PCMInstrument : BaseLeafInstrument {

        public UInt16 swar;
        public UInt16 swav;

        public PCMInstrument(BinaryReader r) {
            swar = r.ReadUInt16();
            swav = r.ReadUInt16();
            parseFields(r);
        }
        
    }
}