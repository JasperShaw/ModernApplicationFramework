using System;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal sealed class AllOtherWindowFontAndColorCategory : ToolWindowFontAndColorCategoryBase
    {
        private string CategoryName { get; }

        public AllOtherWindowFontAndColorCategory(string categoryName, Guid categoryGuid)
        {
            CategoryGuid = categoryGuid;
            CategoryName = categoryName;
        }

        public override string GetCategoryName()
        {
            return CategoryName;
        }

        protected override ushort GetPriorityOrder()
        {
            return 3;
        }
    }
}