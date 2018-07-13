using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal
{
    internal class ListSelectionState
    {
        public ItemDataSource CaretItemDataSource { get; set; }
        public IList<ItemDataSource> SelectedItemDataSources { get; }

        public ListSelectionState()
        {
            SelectedItemDataSources = new List<ItemDataSource>();
        }
    }
}