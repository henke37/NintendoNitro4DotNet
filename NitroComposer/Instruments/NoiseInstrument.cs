using HenkesUtils;
using System.IO;

namespace NitroComposer.Instruments {
    public class NoiseInstrument : Instrument {

        public NoiseInstrument(BinaryReader r) {
            r.Skip(4);
            parseRecord(r);
        }
    }
}