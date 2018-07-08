using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Imaging
{
    internal static class ExtensionMethods
    {
        internal static ImageMoniker ToInternalType(this Interop.ImageMoniker moniker)
        {
            return new ImageMoniker(moniker.CatalogGuid, moniker.Id);
        }
    }
}
