using System;

namespace CreateMask.Containers
{
    public class CheckForReleaseArgs
    {
        public Version CurrentVersion { get; set; }
        public Action<ReleaseInfo> OnNewReleaseCallBack { get; set; }
    }
}
