using System.Text;

namespace Nitro.Graphics {
	public class BGR555 {
		public const uint DataSize=2;
		public byte R, G, B;

		public int NormalizedR { get => ScaleUp(R); }
		public int NormalizedG { get => ScaleUp(G); }
		public int NormalizedB { get => ScaleUp(B); }

		public static explicit operator BGR555(ushort v) {
			var c = new BGR555();
			c.R = (byte)(v & 0x1F);
			c.G = (byte)((v>>5) & 0x1F);
			c.B = (byte)((v>>10) & 0x1F);
			return c;
		}

		private static byte ScaleUp(int v) {
			return (byte)((v << 3) | (v >> 2));
		}

		public override string ToString() {
			var sb = new StringBuilder();

			sb.Append(NormalizedR);
			sb.Append(", ");
			sb.Append(NormalizedG);
			sb.Append(", ");
			sb.Append(NormalizedB);

			return sb.ToString();
		}
	}
}