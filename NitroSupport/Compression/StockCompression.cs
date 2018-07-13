using System;
using System.IO;
using System.Text;
using HenkesUtils;

namespace Nitro.Compression {
	static class StockCompression {

		public static byte[] DecompressArray(byte[] inArr) {
			var decStream = OpenCompressedStream(new MemoryStream(inArr, false));
			var outArr = new byte[decStream.Length];
			decStream.Read(outArr, 0, (int)decStream.Length);
			return outArr;
		}

		public static Stream OpenCompressedStream(Stream baseStream) {
			using(var r = new BinaryReader(baseStream, Encoding.Default, true)) {
				byte type = r.ReadByte();
				byte variant = (byte)(type & 0x0F);
				type >>= 4;
				uint length = r.Read3ByteUInt();

				Stream decompressedStream;

				switch(type & ~8) {
					case 0:
						decompressedStream = baseStream;
						break;
					case 3:
						decompressedStream = new RLEDecoderStream(baseStream, length);
						break;

					default:
						throw new InvalidDataException("Unknown compression type");
				}

				if((type & 8)!=0) {

				}
				return decompressedStream;
			}

		}
	}
}
