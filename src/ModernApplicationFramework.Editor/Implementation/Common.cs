﻿using System.Diagnostics;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal static class Common
    {
        internal static bool OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;
            Process.Start(url);
            return true;
        }
    }
}