using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontAndColorDefaults))]
    internal sealed class ImmediateWindowFontAndColorCategory : ToolWindowFontAndColorCategoryBase
    {
        public ImmediateWindowFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidImmediateWindow;
        }

        public override string GetCategoryName()
        {
            //TODO String
            return "Immediate Window";
        }

        protected override ushort GetPriorityOrder()
        {
            return 3;
        }
    }
}