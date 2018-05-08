using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateMask.Containers
{
    public class CheckForReleaseArgs
    {
        public string Owner { get; set; }
        public string Repository { get; set; }
        public Version CurrentVersion { get; set; }
        public Action<ReleaseInfo> OnNewReleaseCallBack { get; set; }
    }
}
