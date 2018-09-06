using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    internal static class LightUps
    {
        internal static readonly Type BclAsyncLocalType;
        internal static readonly bool IsRunContinuationsAsynchronouslySupported;
        internal static readonly TaskCreationOptions RunContinuationsAsynchronously;
        private static readonly Version Windows8Version = new Version(6, 2, 9200);
        internal const bool ForceWindows7Mode = false;

        internal static bool IsWindows8OrLater => !ForceWindows7Mode
                                                  && Environment.OSVersion.Platform == PlatformID.Win32NT
                                                  && Environment.OSVersion.Version >= Windows8Version;

        static LightUps()
        {
            IsRunContinuationsAsynchronouslySupported =
                Enum.TryParse("RunContinuationsAsynchronously", out RunContinuationsAsynchronously);
            BclAsyncLocalType = Type.GetType("System.Threading.AsyncLocal`1");
        }
    }
}
