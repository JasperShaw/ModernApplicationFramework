using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Guid(CategoryGuids.PrinterTextManager)]
    internal sealed class PrinterTextManagerFontAndColorCategory : TextManagerFontAndColorCategory
    {
        public PrinterTextManagerFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidPrinterTextManager;
            RaiseTextManagerEvents = false;
        }
    }
}