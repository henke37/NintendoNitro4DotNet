using System.IO;

namespace Nitro.Compression {
	internal class DeltaCodedStream : Stream {
		private Stream baseStream;

		public DeltaCodedStream(Stream decompressedStream) {
			this.baseStream = decompressedStream;
		}

		public override bool CanRead => baseStream.CanRead;
		public override bool CanSeek => baseStream.CanSeek;
		public override bool CanWrite => baseStream.CanWrite;
		public override bool CanTimeout => baseStream.CanTimeout;
		public override int ReadTimeout { get => baseStream.ReadTimeout; set => baseStream.ReadTimeout = value; }
		public override int WriteTimeout { get => baseStream.WriteTimeout; set => baseStream.WriteTimeout = value; }

		public override long Length => baseStream.Length;

		public override long Position { get => baseStream.Position;
			set => Seek(value,SeekOrigin.Begin); }

		public override void Flush() {
			baseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count) {
			throw new System.NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin) {
			throw new System.NotImplementedException();
		}

		public override void SetLength(long value) {
			baseStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new System.NotImplementedException();
		}
	}
}