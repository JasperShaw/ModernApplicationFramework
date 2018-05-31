namespace ModernApplicationFramework.Modules.Toolbox.Design
{
    internal class DesignTimeToolboxViewModel : ToolboxViewModel
    {
        public DesignTimeToolboxViewModel()
            : base(null, null)
        {
            Categories.Add(ToolboxItemCategory.DefaultCategory);
        }
    }
}
