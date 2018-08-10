using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    [Guid(CategoryGuids.ToolWindowGroup)]
    internal class ToolWindowGroup : IFontAndColorGroup
    {
        public int GetCount()
        {
            return 4;
        }

        public FcPriority GetPriority()
        {
            return FcPriority.Clients;
        }

        public string GetGroupName()
        {
            //TODO: Text
            return "All Tool Windows";
        }

        public Guid GetCategory(int category)
        {
            switch (category)
            {
                case 0:
                    return CategoryGuids.GuidCommandWindow;
                case 1:
                    return CategoryGuids.GuidImmediateWindow;
                case 2:
                    return CategoryGuids.GuidOutputWindow;
                case 3:
                    return CategoryGuids.GuidFindResultsWindow;
            }
            return Guid.Empty;
        }
    }
}