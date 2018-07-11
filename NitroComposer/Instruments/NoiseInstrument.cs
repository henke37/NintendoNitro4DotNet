using HenkesUtils;
using System.IO;

namespace Nitro.Composer.Instruments {
    public class NoiseInstrument : BaseLeafInstrument {

        public NoiseInstrument(BinaryReader r) {
            r.Skip(4);
            parseFields(r);
        }
    }
}