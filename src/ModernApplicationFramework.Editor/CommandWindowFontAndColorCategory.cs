using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    [Guid(CategoryGuids.CommandWindow)]
    internal sealed class CommandWindowFontAndColorCategory : ToolWindowFontAndColorCategoryBase
    {
        public CommandWindowFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidCommandWindow;
        }

        public override string GetCategoryName()
        {
            //TODO Text
            return "Command Window";
        }

        protected override ushort GetPriorityOrder()
        {
            return 1;
        }
    }
}