namespace Nitro.Composer.SequenceCommands.Var {
	public class ProgramChangeVarCommand : BaseSequenceCommand {
		public uint ProgramVar;

		public ProgramChangeVarCommand(uint ProgramVar) {
			this.ProgramVar = ProgramVar;
		}
	}
}
