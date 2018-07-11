namespace Nitro.Composer.SequenceCommands {
    public class VarCommand : BaseSequenceCommand {
        public Operator Op;
        public byte Variable;
        public uint Operand;

        public VarCommand(Operator Op, byte variable, uint operand) {
            this.Op = Op;
            this.Variable = variable;
            this.Operand = operand;
        }

        public enum Operator : byte {
            ASSIGN=0xB0,
            ADD,
            SUB,
            MUL,
            DIV,
            SHIFT,
            RAND,
            EQU=0xB8,
            GTE,
            GT,
            LTE,
            LT,
            NEQ
        }
    }
}
