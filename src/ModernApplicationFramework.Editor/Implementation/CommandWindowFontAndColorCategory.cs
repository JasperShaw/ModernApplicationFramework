using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontAndColorDefaults))]
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