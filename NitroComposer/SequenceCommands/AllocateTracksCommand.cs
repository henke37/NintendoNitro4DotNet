using System;

namespace Henke37.Nitro.Composer.SequenceCommands {
    public class AllocateTracksCommand : BaseSequenceCommand {
        public UInt16 Tracks;
        public AllocateTracksCommand(UInt16 tracks) {
            Tracks = tracks;
        }
    }
}
