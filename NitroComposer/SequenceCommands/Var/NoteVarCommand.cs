using Nitro.Composer.SequenceCommands;

namespace Nitro.Composer.SequenceCommands.Var {
	public class NoteVarCommand : BaseSequenceCommand {
		public uint Note;
		public uint Velocity;
		public uint DurationVar;

		public NoteVarCommand(uint note, uint vel, uint durationVar) {
			Note = note;
			Velocity = vel;
			DurationVar = durationVar;
		}
	}
}
