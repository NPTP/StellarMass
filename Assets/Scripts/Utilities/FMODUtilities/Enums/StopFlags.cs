using System;

namespace Summoner.Utilities.FMODUtilities.Enums
{
    [Flags]
    public enum StopFlags
    {
        AllowFadeout = 1 << 0,
        Immediate = 1 << 1,
        Release = 1 << 2,
        ClearHandle = 1 << 3
    }
}
