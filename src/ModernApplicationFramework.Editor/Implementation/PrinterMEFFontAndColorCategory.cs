using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
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