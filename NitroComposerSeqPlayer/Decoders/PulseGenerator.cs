using System;
using System.Collections.Generic;

namespace NitroComposerPlayer.Decoders {
	class PulseGenerator : BaseGenerator {
		private int[] currentPulseWidthTable;

		public PulseGenerator(int pulseWidth) {
			this.pulseWidth = pulseWidth;
		}

		internal override int GetSample() {
			return currentPulseWidthTable[(int)samplePosition % 8];
		}

		private static readonly int[][] pulseWidthLUT = new int[][] {
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF,  0x7FFF},
			new int[] {-0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF, -0x7FFF}
		};

		internal int pulseWidth {
			set {
				if(value == -1) {
					currentPulseWidthTable = null;
					return;
				}
				currentPulseWidthTable = pulseWidthLUT[value];
			}
			get {
				for(int i = 0; i < 8; ++i) {
					if(currentPulseWidthTable == pulseWidthLUT[i]) return i;
				}
				return -1;
			}
		}

		private static string[] dutyMap = new string[] {
			"12.5%", "25.0%", "37.5%", "50.0%",
			"62.5%", "75.0%", "87.5%", "0.0%"
		};

		public override string ToString() {
			return dutyMap[pulseWidth];
		}
	}
}
