﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontAndColorDefaults))]
    internal sealed class ToolTipFontAndColorCategory : ToolWindowFontAndColorCategoryBase
    {
        private List<AllColorableItemInfo> _items;

        public ToolTipFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidToolTip;
        }

        protected override List<AllColorableItemInfo> Items => _items ?? (_items = new List<AllColorableItemInfo>
        {
            //TODO: Text
            FontsAndColorsHelper.GetPlainTextItem("Plain Text", "Plain Text",
                Color.FromRgb(30, 30, 30),
                Colors.Transparent,
                Colors.Black,
                Colors.White)
        });

        public override string GetCategoryName()
        {
            //TODO: Text
            return "Tool Tip";
        }

        public override ushort GetPriority()
        {
            return 2;
        }
    }
}