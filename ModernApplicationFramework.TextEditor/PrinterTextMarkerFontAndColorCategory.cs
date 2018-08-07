using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
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