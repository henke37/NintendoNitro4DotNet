namespace Nitro.Graphics {
	public class BGR555 {
		public const uint DataSize=2;
		public byte R, G, B;

		public static explicit operator BGR555(ushort v) {
			var c = new BGR555();
			c.R = ScaleUp(v & 0x1F);
			c.G = ScaleUp((v>>5) & 0x1F);
			c.B = ScaleUp((v>>10) & 0x1F);
			return c;
		}

		private static byte ScaleUp(int v) {
			return (byte)((v << 3) | (v >> 2));
		}
	}
}