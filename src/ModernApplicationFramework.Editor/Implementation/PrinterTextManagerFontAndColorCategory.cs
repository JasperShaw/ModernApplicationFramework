using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
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