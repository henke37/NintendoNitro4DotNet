namespace Nitro.Composer.SequenceCommands.Var {
	public class LoopStartVarCommand : BaseSequenceCommand {
		public byte LoopCountVar;
		public LoopStartVarCommand(byte countVar) {
			LoopCountVar = countVar;
		}
	}
}
