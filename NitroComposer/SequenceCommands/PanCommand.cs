﻿namespace NitroComposer.SequenceCommands {
    public class PanCommand : BaseSequenceCommand {
        public byte Pan;

        public PanCommand(byte pan) {
            this.Pan = pan;
        }
    }
}