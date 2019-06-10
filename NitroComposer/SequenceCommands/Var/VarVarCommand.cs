namespace Henke37.Nitro.Composer.SequenceCommands.Var {
	public class VarVarCommand : BaseSequenceCommand {
		public VarCommand.Operator Op;
		public byte Variable1;
		public byte Variable2;

		public VarVarCommand(VarCommand.Operator Op, byte variable1, byte variable2) {
			this.Op = Op;
			this.Variable1 = variable1;
			this.Variable2 = variable2;
		}
	}
}
