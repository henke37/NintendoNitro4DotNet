using Henke37.IOUtils;
using System;
using System.IO;

namespace Henke37.Nitro.Composer.Instruments {
    public abstract class Instrument {
        private const int RECORD_LENGTH=10;

        internal static Instrument parseRecord(byte instrumentType, Stream stream) {
            using(var r=new BinaryReader(stream)) {
                return parseRecord(instrumentType, r);
            }
        }

        internal static Instrument parseRecord(byte instrumentType, BinaryReader r) {
            switch(instrumentType) {
                case 0:
                    r.Skip(RECORD_LENGTH);
                    return null;
                case 1:
                    return new PCMInstrument(r);
                case 2:
                    return new PulseInstrument(r);
                case 3:
                    return new NoiseInstrument(r);
                case 16:
                    return new DrumkitInstrument(r);
                case 17:
                    return new SplitInstrument(r);
                default:
                    throw new NotSupportedException();
            }
        }

        public abstract BaseLeafInstrument leafInstrumentForNote(byte note);
    }
}