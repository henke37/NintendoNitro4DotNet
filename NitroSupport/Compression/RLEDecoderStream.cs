using System;
using System.IO;
using System.Text;

namespace Nitro.Compression {
	public class RLEDecoderStream : Stream {

		private Stream BaseStream;
		private BinaryReader Reader;
		private long DecompressedLength;
		private long Progress;

		private Mode CurrentMode;
		private int RemaingRunLength;
		private byte SourceByte;

		public RLEDecoderStream(Stream baseStream, long DecompressedLength, bool leaveOpen = true) {
			BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
			if(!baseStream.CanRead) throw new ArgumentException("Stream has to be readable!", nameof(baseStream));
			Reader = new BinaryReader(baseStream, Encoding.Default, leaveOpen);

			this.DecompressedLength = DecompressedLength;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			int bytesWritten = 0;
			while(Progress < DecompressedLength && bytesWritten < count) {
				if(RemaingRunLength==0) ReadNextHeader();
				
				int startOffset = offset + bytesWritten;
				int bytesToWrite = count - bytesWritten;
				if(bytesToWrite > RemaingRunLength) bytesToWrite = RemaingRunLength;

				switch(CurrentMode) {
					case Mode.NonCompressedRun:
						bytesWritten += BaseStream.Read(buffer, startOffset, bytesToWrite);
						break;
					case Mode.CompressedRun:
						bytesWritten += HandleCompressedRun(buffer, startOffset, bytesToWrite);
						break;
				}

				RemaingRunLength -= bytesToWrite;
				Progress += bytesToWrite;
			}

			return bytesWritten;
		}

		private int HandleCompressedRun(byte[] buffer, int offset, int bytesToWrite) {
			int endOffset = offset + bytesToWrite;
			for(;offset<endOffset;++offset) {
				buffer[offset] = SourceByte;
			}
			return bytesToWrite;
		}

		private void ReadNextHeader() {
			byte flag = Reader.ReadByte();
			RemaingRunLength = flag & 0x7F;

			CurrentMode = (Mode)(flag & 0x80);

			if(CurrentMode==Mode.CompressedRun) {
				SourceByte = Reader.ReadByte();
				RemaingRunLength += 3;
			} else {
				RemaingRunLength += 1;
			}
		}

		private void SkipAhead(long Target) {
			while(Progress < Target) {
				if(RemaingRunLength == 0) ReadNextHeader();

				if(Progress + RemaingRunLength >= Target) {
					//bigger skip, discard entire run
					Progress += RemaingRunLength;
				} else {
					//tiny skip, fits in current run
					RemaingRunLength -= (int)(Target - Progress);
					Progress = Target;
					break;
				}
			}
		}

		private enum Mode {
			CompressedRun = 0x80,//Clever trick to simplify header parsing
			NonCompressedRun = 0x00
		}

		#region Contract cruff

		public override bool CanRead => true;
		public override bool CanSeek => BaseStream.CanSeek;
		public override bool CanWrite => false;

		public override long Length => DecompressedLength;

		public override long Position {
			get => Progress;
			set => Seek(value, SeekOrigin.Begin);
		}

		public override long Seek(long offset, SeekOrigin origin) {
			if(origin == SeekOrigin.Current) offset += Progress;
			else if(origin == SeekOrigin.End) offset = Progress - offset;

			if(offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
			if(offset >= DecompressedLength) throw new ArgumentOutOfRangeException(nameof(offset));

			if(offset < Progress) {
				BaseStream.Seek(0, SeekOrigin.Begin);
				Progress = 0;
			}

			SkipAhead(offset);

			return Progress;
		}

		public override int ReadTimeout { get => BaseStream.ReadTimeout; set => BaseStream.ReadTimeout = value; }
		public override int WriteTimeout { get => BaseStream.WriteTimeout; set => BaseStream.WriteTimeout = value; }

		public override void Flush() {
			throw new NotSupportedException();
		}

		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
