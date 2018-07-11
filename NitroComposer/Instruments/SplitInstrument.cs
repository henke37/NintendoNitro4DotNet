using HenkesUtils;
using System;
using System.IO;

namespace Nitro.Composer.Instruments {
    public class SplitInstrument : MetaInstrument {

        public SplitInstrument(BinaryReader r) {
            var regionEnds=r.ReadBytes(8);

            byte start = 0;

            foreach(var end in regionEnds) {
                if(end == 0) break;

                var region = new InstrumentRegion();
                region.lowEnd = start;
                region.highEnd = end;

                byte instrumentType = r.ReadByte();
                r.Skip(1);

                region.subInstrument = (BaseLeafInstrument)Instrument.parseRecord(instrumentType, r);
                regions.Add(region);

                start = (byte)(end + 1);
            }
        }
    }
}
