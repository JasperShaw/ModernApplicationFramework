using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Guid(CategoryGuids.PrinterTextMarker)]
    internal sealed class PrinterTextMarkerFontAndColorCategory : TextMarkerFontAndColorCategory
    {
        public PrinterTextMarkerFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidPrinterTextMarker;
            RaiseTextManagerEvents = false;
        }
    }
}