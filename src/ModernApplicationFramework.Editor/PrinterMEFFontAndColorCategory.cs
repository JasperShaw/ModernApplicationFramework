using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    [Guid(CategoryGuids.PrinterMef)]
    internal sealed class PrinterMefFontAndColorCategory : MefFontAndColorCategory
    {
        public PrinterMefFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidPrinterMef;
            RaiseTextManagerEvents = false;
        }
    }
}