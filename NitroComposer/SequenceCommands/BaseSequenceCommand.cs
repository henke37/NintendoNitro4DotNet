namespace Henke37.Nitro.Composer.SequenceCommands {
    public abstract class BaseSequenceCommand {
		public bool Conditional;

		internal virtual bool EndsFlow => false;
    }
}
