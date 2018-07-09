using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    [Export(typeof(IToolboxItemPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ChooseItemsPage2 : ChooseItemsPage
    {
        public ChooseItemsPage2()
        {
            DisplayName = "Test2";
        }
    }

    [Export(typeof(IToolboxItemPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ChooseItemsPage1 : ChooseItemsPage
    {
        public ChooseItemsPage1()
        {
            DisplayName = "Test1";
        }
    }
}
