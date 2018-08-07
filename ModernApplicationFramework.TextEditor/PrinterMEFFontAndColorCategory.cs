using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
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