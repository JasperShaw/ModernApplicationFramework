using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    [Guid("47724E70-AF55-48fb-A928-BB161C1D0C05")]
    internal class PrinterGroup : IFontAndColorGroup
    {
        public Guid GetCategory(int iCategory)
        {
            if (iCategory == 0)
                return CategoryGuids.GuidPrinterTextMarker;
            if (iCategory == 1)
                return CategoryGuids.GuidPrinterMef;
            if (iCategory == 2)
                return CategoryGuids.GuidPrinterTextManager;
            if (iCategory == 3)
                return CategoryGuids.GuidPrinterTextMarker;
            return Guid.Empty;
        }

        public int GetCount()
        {
            return 4;
        }

        public string GetGroupName()
        {
            //Todo Text
            return "Printer Group";
        }

        public FcPriority GetPriority()
        {
            return FcPriority.Environment;
        }
    }
}