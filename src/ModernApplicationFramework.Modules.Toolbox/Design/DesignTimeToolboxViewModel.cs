namespace ModernApplicationFramework.Modules.Toolbox.Design
{
    internal class DesignTimeToolboxViewModel : ToolboxViewModel
    {
        public DesignTimeToolboxViewModel()
            : base(null)
        {

            var cat1 = new ToolboxItemCategory(null, "Test 1");
            var cat2 = new ToolboxItemCategory(null, "Test 2");

            var child1 = new ToolboxItemEx(typeof(object), cat1, "Child 1");
            var child2 = new ToolboxItemEx(typeof(object),cat1, "Child 2");

            
            Categories.Add(cat1);
            Categories.Add(cat2);


            //Items.Add(new ToolboxItemViewModel(new ToolboxItem { Name = "Foo", Category = "General" }));
            //Items.Add(new ToolboxItemViewModel(new ToolboxItem { Name = "Bar", Category = "General" }));
            //Items.Add(new ToolboxItemViewModel(new ToolboxItem { Name = "Baz", Category = "Misc" }));
        }
    }
}
