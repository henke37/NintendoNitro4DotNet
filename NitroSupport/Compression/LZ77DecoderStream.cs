using System.IO;

namespace Nitro.Compression {
	public class LZ77DecoderStream : Stream {
		private Stream baseStream;
		private int DecompressedLength;
		private bool longLengths;

		private BinaryReader Reader;

		private int RunLength;
		private int RunDistance;

		public LZ77DecoderStream(Stream baseStream, int DecompressedLength, byte variant) {
			this.baseStream = baseStream;
			this.DecompressedLength = DecompressedLength;
			this.longLengths = variant != 0;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			throw new System.NotImplementedException();
		}

		private void ReadReference() {
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
		}

		#region Contract cruff

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => false;
		public override bool CanTimeout => baseStream.CanTimeout;
		public override int ReadTimeout { get => baseStream.ReadTimeout; set => baseStream.ReadTimeout = value; }

		public override long Length => DecompressedLength;

		public override long Position {
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		public override void Flush() {
			throw new System.NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin) {
			throw new System.NotImplementedException();
		}

		public override void SetLength(long value) {
			throw new System.NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new System.NotSupportedException();
		}
		#endregion
	}
}