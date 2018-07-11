namespace Nitro.Composer.SequenceCommands.Rand {
	public class VarRandCommand : BaseSequenceCommand {
		public VarCommand.Operator Op;
		public byte Variable1;
		public uint OperandMin, OperandMax;

		public VarRandCommand(VarCommand.Operator Op, byte variable1, uint min, uint max) {
			this.Op = Op;
			this.Variable1 = variable1;
			this.OperandMin = min;
			this.OperandMax = max;
		}
	}
}
