using Henke37.Nitro.Composer.SequenceCommands;

namespace Henke37.Nitro.Composer.SequenceCommands.Var {
	public class NoteVarCommand : BaseSequenceCommand {
		public byte Note;
		public uint Velocity;
		public uint DurationVar;

		public NoteVarCommand(byte note, uint vel, uint durationVar) {
			Note = note;
			Velocity = vel;
			DurationVar = durationVar;
		}
	}
}
