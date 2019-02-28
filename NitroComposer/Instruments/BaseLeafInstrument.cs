using System.IO;

namespace Nitro.Composer.Instruments {
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

		public override string ToString() {
			return string.Format("{0} {1:X} {2:X} {3:X} {4:X}",BaseNote,Attack,Decay,Sustain,Release);
		}
	}
}
