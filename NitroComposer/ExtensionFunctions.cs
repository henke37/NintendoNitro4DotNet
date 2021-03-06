﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Henke37.Nitro.Composer {
    static class ExtensionFunctions {
        public static uint ReadMIDIVarLen(this BinaryReader r) {
            uint value = 0;

            for(; ; ) {

                byte b = r.ReadByte();

                value <<= 7;

                value |= (byte)(b & 0x7F);

                if((b & 0x80) != 0x80) break;
            }

            return value;
        }
    }
}
