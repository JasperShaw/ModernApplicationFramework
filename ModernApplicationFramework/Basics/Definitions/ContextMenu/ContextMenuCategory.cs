namespace ModernApplicationFramework.Basics.Definitions.ContextMenu
{
    public class ContextMenuCategory
    {
        public static ContextMenuCategory OtherContextMenusCategory = new ContextMenuCategory("Other Context Menus");

        public string CategoryName { get; }

        public ContextMenuCategory(string categoryName) : this(null, categoryName)
        {
        }

        public ContextMenuCategory(ContextMenuCategory parentCategory, string categoryName)
        {
            CategoryName = parentCategory == null ? categoryName : $"{parentCategory.CategoryName} | {categoryName}";
        }
    }
}