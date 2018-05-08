using System;
using System.Diagnostics;

namespace CreateMask.Utilities
{
    public static class Web
    {
        public static void OpenUrlInDefaultBrowser(Uri uri)
        {
            OpenUrlInDefaultBrowser(uri.AbsoluteUri);
        }

        public static void OpenUrlInDefaultBrowser(string url)
        {
            Process.Start(url);
        }
    }
}
