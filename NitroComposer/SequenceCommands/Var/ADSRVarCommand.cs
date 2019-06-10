namespace Henke37.Nitro.Composer.SequenceCommands.Var {
	public class ADSRVarCommand : BaseSequenceCommand {
		public byte Var;
		public ADSRCommand.EnvPos envPos;

		public ADSRVarCommand(ADSRCommand.EnvPos envPos, byte var) {
			this.envPos = envPos;
			Var = var;
		}
	}
}
