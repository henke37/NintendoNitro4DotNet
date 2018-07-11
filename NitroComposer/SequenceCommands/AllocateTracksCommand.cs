using System;

namespace Nitro.Composer.SequenceCommands {
    public class AllocateTracksCommand : BaseSequenceCommand {
        public UInt16 Tracks;
        public AllocateTracksCommand(UInt16 tracks) {
            Tracks = tracks;
        }
    }
}
