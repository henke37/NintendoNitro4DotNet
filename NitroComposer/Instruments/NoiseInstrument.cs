using Henke37.IOUtils;
using System.IO;

namespace Henke37.Nitro.Composer.Instruments {
    public class NoiseInstrument : BaseLeafInstrument {

        public NoiseInstrument(BinaryReader r) {
            r.Skip(4);
            parseFields(r);
        }
    }
}