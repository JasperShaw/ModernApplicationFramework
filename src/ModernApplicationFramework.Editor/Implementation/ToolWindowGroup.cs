using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontAndColorGroup))]
    internal class ToolWindowGroup : IFontAndColorGroup
    {
        public Guid GroupGuid => CategoryGuids.GuidToolWindowGroup;

        public int GetCount()
        {
            return 4;
        }

        public ushort GetPriority()
        {
            return 3;
        }

        public string GetGroupName()
        {
            //TODO: Text
            return "[All Tool Windows]";
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