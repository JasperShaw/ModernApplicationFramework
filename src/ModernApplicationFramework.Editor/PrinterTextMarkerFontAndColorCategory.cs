using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
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