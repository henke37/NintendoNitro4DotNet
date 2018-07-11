using HenkesUtils;
using System.IO;

namespace Nitro.Composer.Instruments {
    public class DrumkitInstrument : MetaInstrument {

        public DrumkitInstrument(BinaryReader r) {
            byte low = r.ReadByte();
            byte high = r.ReadByte();

            if(high < low) throw new InvalidDataException("High must be higher than low!");

            for(byte note=low;note<=high;++note) {
                var region = new InstrumentRegion();
                region.lowEnd = note;
                region.highEnd = note;

                byte instrumentType = r.ReadByte();
                r.Skip(1);

                region.subInstrument = (BaseLeafInstrument)Instrument.parseRecord(instrumentType, r);

                regions.Add(region);
            }
        }
    }
}