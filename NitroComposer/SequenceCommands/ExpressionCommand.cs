﻿namespace NitroComposer.SequenceCommands {
    public class ExpressionCommand : BaseSequenceCommand {
        public byte Value;
        public ExpressionCommand(byte val) {
            Value = val;
        }
    }
}