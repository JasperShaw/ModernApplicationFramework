using System.Collections.Generic;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Guid(CategoryGuids.FindResultsWindow)]
    internal sealed class FindResultsWindowFontAndColorCategory : ToolWindowFontAndColorCategoryBase
    {
        private List<AllColorableItemInfo> _items;

        public FindResultsWindowFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidFindResultsWindow;
        }

        protected override List<AllColorableItemInfo> Items
        {
            get
            {
                if (_items != null)
                    return _items;
                base.Items.Add(GetItemForMefName("outlining.collapsehintadornment"));
                base.Items.Add(GetItemForMefName("outlining.verticalrule"));
                base.Items.Add(GetItemForMefName("outlining.square"));
                _items = base.Items;
                return _items;
            }
        }

        protected override bool IsLegacyMarker(string item)
        {
            if (item != "Current List Location" && item != "Collapsible Text (Collapsed)")
                return item == "Collapsible Text (Expanded)";
            return true;
        }

        public override string GetCategoryName()
        {
            return "Find Search Result";
        }

        protected override ushort GetPriorityOrder()
        {
            return 2;
        }
    }
}