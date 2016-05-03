using System;
using System.IO;

namespace ModernApplicationFramework.Configuration
{
    public static class ProductConfiguration
    {
        public static string ProductName => "Modern Application Framework";

        public static string AppdataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProductName);
    }
}
