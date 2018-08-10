using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    [Guid(CategoryGuids.ImmediateWindow)]
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