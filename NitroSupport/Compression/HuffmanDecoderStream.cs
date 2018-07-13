using System;
using System.IO;
using System.Text;
using HenkesUtils;

namespace Nitro.Compression {
	public class HuffmanDecoderStream : Stream {
		private Stream baseStream;
		private int DecompressedLength;
		private byte variant;
		private int Progress;

		private BinaryReader reader;

		public HuffmanDecoderStream(Stream baseStream, int DecompressedLength, byte variant) {
			this.baseStream = baseStream;
			this.DecompressedLength = DecompressedLength;
			this.variant = variant;

			reader = new BinaryReader(baseStream, Encoding.Default, true);

			LoadTree(0);
		}

		private TreeNode LoadTree(int startPos) {
			reader.Seek(startPos);

			byte flags = reader.ReadByte();
			bool leftIsData = (flags & 0x80) != 0;
			bool rightIsData = (flags & 0x40) != 0;
			byte depth = (byte)(flags & 0x1F);

			int childStartOffset = (startPos & ~1) + depth * 2 + 2;

			var parent = new TreeNode();

			if(leftIsData) {
				reader.Seek(childStartOffset);
				parent.left = new TreeNode(reader.ReadByte());
			} else {
				parent.left = LoadTree(childStartOffset);
			}

			if(rightIsData) {
				reader.Seek(childStartOffset + 1);
				parent.right = new TreeNode(reader.ReadByte());
			} else {
				parent.right = LoadTree(childStartOffset + 1);
			}

			return parent;
		}

		private byte ReadByteInternal() {
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count) {
			if(count + Progress > DecompressedLength) {
				count = DecompressedLength - Progress;
			}

			int endOffset = offset + count;
			for(; offset < endOffset; ++offset) {
				buffer[offset] = ReadByteInternal();
			}

			Progress += count;

			return count;
		}

		public override int ReadByte() {
			if(Progress >= DecompressedLength) return -1;
			Progress++;
			return ReadByteInternal();
		}

		private class TreeNode {
			public byte data;

			public TreeNode left, right;

			public TreeNode() { }

			public TreeNode(byte data) {
				this.data = data;
			}
		}


		#region Contract cruff

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => false;

		public override long Length => DecompressedLength;

		public override long Position { get => Progress; set => throw new System.NotImplementedException(); }

		public override long Seek(long offset, SeekOrigin origin) {
			throw new System.NotImplementedException();
		}

		public override void Flush() {
			throw new System.NotSupportedException();
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