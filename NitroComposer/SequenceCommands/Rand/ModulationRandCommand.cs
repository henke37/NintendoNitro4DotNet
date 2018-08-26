namespace Nitro.Composer.SequenceCommands.Rand {
	public class ModulationRandCommand : BaseSequenceCommand {
		public short Min, Max;
		public ModulationCommand.ModType Type;

		public ModulationRandCommand(ModulationCommand.ModType type, short min, short max) {
			Type = type;
			Min = min;
			Max = max;
		}
	}
}