﻿using HenkesUtils;
using System;
using System.IO;

namespace NitroComposerPlayer.Decoders {
	internal class ADPCMDecoder : BaseSampleDecoder {

		private int predictor;
		private int stepIndex;

		private int currentPos;
		private int storedNibble;

		public ADPCMDecoder() {

		}

		public override void Init(BinaryReader reader) {
			this.reader = reader;
			Reset();
		}

		private void Reset() {
			reader.Seek(0);
			currentPos = -1;//Startup HACK: we are just before the first sample
			predictor = reader.ReadUInt16();
			stepIndex = reader.ReadUInt16();
		}

		internal override int GetSample() {
			if(currentPos == samplePosition) {
				return predictor;
			}

			if(currentPos>samplePosition) {
				Reset();
			}

			if((currentPos % 2) == 1) {//not the same as !=0, should not handle start up position of -1
				parseNibble(storedNibble);
			}

			for(; currentPos+2<=samplePosition;) {
				var nibble = reader.ReadByte();
				parseNibble(nibble | 0xF);
				parseNibble(nibble >> 8);
			}

			if(currentPos == samplePosition) {
				return predictor;
			}

			if((currentPos % 2)!=0) {//not the same as ==1, have to handle start up position of -1
				var nibble = reader.ReadByte();
				parseNibble(nibble | 0xF);
				storedNibble = nibble >> 8;
			} else {
				parseNibble(storedNibble);
			}
			return predictor;
		}

		private void parseNibble(int nibble) {
			int step = stepTable[stepIndex];

			stepIndex += indexTable[nibble];

			if(stepIndex < 0) {
				stepIndex = 0;
			} else if(stepIndex > 88) {
				stepIndex = 88;
			}

			int diff = step >> 3;
			if((nibble & 1) != 0) diff += step >> 2;
			if((nibble & 2) != 0) diff += step >> 1;
			if((nibble & 4) != 0) diff += step;

			if((nibble & 8) != 0) {
				predictor -= diff;
				if(predictor < -32767) {
					predictor = -32767;
				}
			} else {
				predictor += diff;
				if(predictor > 32767) {
					predictor = 32767;
				}
			}
			currentPos++;
		}

		private static readonly int[] stepTable = new int[] {
			7, 8, 9, 10, 11, 12, 13, 14, 16, 17,
			19, 21, 23, 25, 28, 31, 34, 37, 41, 45,
			50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
			130, 143, 157, 173, 190, 209, 230, 253, 279, 307,
			337, 371, 408, 449, 494, 544, 598, 658, 724, 796,
			876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066,
			2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358,
			5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
			15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
		};

		private static readonly int[] indexTable = new int[] {
			-1, -1, -1, -1, 2, 4, 6, 8,
			-1, -1, -1, -1, 2, 4, 6, 8
		};
	}
}