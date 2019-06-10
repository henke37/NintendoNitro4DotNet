namespace Henke37.Nitro.Composer.SequenceCommands.Rand {
	public class VarRandCommand : BaseSequenceCommand {
		public VarCommand.Operator Op;
		public byte Variable;
		public short OperandMin, OperandMax;

		public VarRandCommand(VarCommand.Operator Op, byte variable, short min, short max) {
			this.Op = Op;
			this.Variable = variable;
			this.OperandMin = min;
			this.OperandMax = max;
		}
	}
}
