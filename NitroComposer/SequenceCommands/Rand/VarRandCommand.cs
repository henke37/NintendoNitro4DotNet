namespace Nitro.Composer.SequenceCommands.Rand {
	public class VarRandCommand : BaseSequenceCommand {
		public VarCommand.Operator Op;
		public byte Variable;
		public uint OperandMin, OperandMax;

		public VarRandCommand(VarCommand.Operator Op, byte variable, uint min, uint max) {
			this.Op = Op;
			this.Variable = variable;
			this.OperandMin = min;
			this.OperandMax = max;
		}
	}
}
