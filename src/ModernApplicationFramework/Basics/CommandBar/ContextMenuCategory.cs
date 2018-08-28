using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.CommandBar
{
    /// <summary>
    /// Information to categorize a context menu
    /// </summary>
    public class ContextMenuCategory
    {
        /// <summary>
        /// Returns a default context menu category
        /// </summary>
        public static ContextMenuCategory OtherContextMenusCategory => new ContextMenuCategory(CommonUI_Resources.OtherContextMenusCategory);

        /// <summary>
        /// The full localized category name
        /// </summary>
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