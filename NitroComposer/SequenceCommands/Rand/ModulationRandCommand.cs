namespace Nitro.Composer.SequenceCommands.Rand {
	public class ModulationRandCommand : BaseSequenceCommand {
		public uint Min, Max;
		public ModulationCommand.ModType Type;

		public ModulationRandCommand(ModulationCommand.ModType type, uint min, uint max) {
			Type = type;
			Min = min;
			Max = max;
		}
	}
}