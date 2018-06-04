using HenkesUtils;
using System.IO;

namespace NitroComposer.Instruments {
    public class NoiseInstrument : BaseLeafInstrument {

        public NoiseInstrument(BinaryReader r) {
            r.Skip(4);
            parseFields(r);
        }
    }
}