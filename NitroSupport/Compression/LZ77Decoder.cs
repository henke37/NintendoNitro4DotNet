using System.IO;
using System.Text;

namespace Henke37.Nitro.Compression {
	public class LZ77Decoder {
		private Stream baseStream;
		private BinaryReader Reader;

		private int DecompressedLength;
		private bool longLengths;

		private byte[] outBuff;

		private int Progress = 0;

		public LZ77Decoder(Stream baseStream, int DecompressedLength, byte variant) {
			this.baseStream = baseStream;
			this.DecompressedLength = DecompressedLength;
			this.longLengths = variant != 0;

			Reader = new BinaryReader(baseStream, Encoding.Default, true);

			outBuff = new byte[DecompressedLength];
		}

		public byte[] Decompress() {
			byte flagByte = 0;
			int bit = 0;

			while(Progress < DecompressedLength) {
				bit = bit >> 1;
				if(bit == 0) {
					flagByte = Reader.ReadByte();
					bit = 0x80;
				}

				if((flagByte & bit) != 0) {
					HandleBackReference();
				} else {
					outBuff[Progress++]=Reader.ReadByte();
				}
			}

			return outBuff;
		}

		private void HandleBackReference() {
			int RunLength;
			int RunDistance;

			if(longLengths) {
				byte x = Reader.ReadByte();
				switch(x >> 4) {
					case 0:
						RunLength = x << 4;
						x = Reader.ReadByte();
						RunLength |= x >> 4;
						RunLength += 0x11;

						RunDistance = (x & 0x0F) << 8;
						RunDistance |= Reader.ReadByte();
						break;

					case 1:
						RunLength = (x & 0x0F) << 8;
						x = Reader.ReadByte();
						RunLength |= x << 4;
						x = Reader.ReadByte();
						RunLength |= x >> 4;
						RunLength += 0x111;

						RunDistance = (x & 0x0F) << 8;
						RunDistance |= Reader.ReadByte();
						break;

					default:
						RunLength = (x >> 4) + 1;
						RunDistance = (x & 0x0F) << 8;
						RunDistance |= Reader.ReadByte();
						break;
				}
			} else {
				byte x = Reader.ReadByte();
				RunLength = x >> 4;
				RunLength += 3;
				RunDistance = (x & 0x0F) << 8;
				RunDistance |= Reader.ReadByte();
			}

			int readPos = Progress - RunDistance - 1;
			if(readPos < 0) throw new InvalidDataException();

			for(var copyIttr=0;copyIttr<RunLength;++copyIttr) {
				outBuff[Progress++] = outBuff[readPos++];
			}
		}

	}
}