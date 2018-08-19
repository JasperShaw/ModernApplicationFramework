using System;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class PrinterGroup : IFontAndColorGroup
    {
        public Guid GroupGuid => CategoryGuids.GuidPrinterGroup;

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

        public ushort GetPriority()
        {
            return 1;
        }
    }
}