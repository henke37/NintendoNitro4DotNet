using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroComposer.Instruments {
    public abstract class MetaInstrument : BaseLeafInstrument {

        protected List<InstrumentRegion> regions;

        protected MetaInstrument() {
            regions = new List<InstrumentRegion>();
        }

        public override BaseLeafInstrument leafInstrumentForNote(byte note) {
            var region = regionForNote(note);
            if(region == null) return null;
            return region.subInstrument;
        }

        private InstrumentRegion regionForNote(byte note) {
            foreach(var region in regions) {
                if(region.MatchesNote(note)) return region;
            }
            return null;
        }

        protected class InstrumentRegion {
            public byte lowEnd;
            public byte highEnd;

            public BaseLeafInstrument subInstrument;

            public bool MatchesNote(byte note) {
                return note >= lowEnd && note <= highEnd;
            }
        }
    }
}
