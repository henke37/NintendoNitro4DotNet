namespace Nitro.Composer.SequenceCommands.Var {
	public class PitchBendVarCommand : BaseSequenceCommand {
		public byte BendVar;
		public bool IsRange;

		public PitchBendVarCommand(byte bendVar, bool range) {
			BendVar = bendVar;
			IsRange = range;
		}
	}
}
