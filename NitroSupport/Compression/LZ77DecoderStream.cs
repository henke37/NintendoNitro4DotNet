using System.IO;

namespace Nitro.Compression {
	public class LZ77DecoderStream : Stream {
		private Stream baseStream;
		private int DecompressedLength;
		private bool longLengths;

		public LZ77DecoderStream(Stream baseStream, int DecompressedLength, byte variant) {
			this.baseStream = baseStream;
			this.DecompressedLength = DecompressedLength;
			this.longLengths = variant!=0;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			throw new System.NotImplementedException();
		}


		#region Contract cruff

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => false;

		public override long Length => DecompressedLength;

		public override long Position { get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException(); }

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