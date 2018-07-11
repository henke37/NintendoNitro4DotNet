namespace Nitro.Composer.SequenceCommands.Var {
	public class ModulationVarCommand : BaseSequenceCommand {
		public byte Var;
		public ModulationCommand.ModType Type;

		public ModulationVarCommand(ModulationCommand.ModType type, byte var) {
			Type = type;
			Var = var;
		}

	}
}
