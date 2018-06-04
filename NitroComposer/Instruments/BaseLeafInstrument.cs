using System.IO;

namespace NitroComposer.Instruments {
    public abstract class BaseLeafInstrument : Instrument {
        public byte BaseNote;

        public byte Attack;
        public byte Decay;
        public byte Sustain;
        public byte Release;

        public byte Pan;

        protected void parseFields(BinaryReader r) {
            BaseNote = r.ReadByte();
            Attack = r.ReadByte();
            Decay = r.ReadByte();
            Sustain = r.ReadByte();
            Release = r.ReadByte();
            Pan = r.ReadByte();
        }

        public override BaseLeafInstrument leafInstrumentForNote(byte note) {
            return this;
        }
    }
}
