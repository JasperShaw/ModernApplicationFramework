﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export((typeof(IFontAndColorDefaults)))]
    internal sealed class OutputWindowFontAndColorCategory : ToolWindowFontAndColorCategoryBase
    {
        private List<AllColorableItemInfo> _items;

        public OutputWindowFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidOutputWindow;
        }

        protected override List<AllColorableItemInfo> Items
        {
            get
            {
                if (_items != null)
                    return _items;
                //TODO Localization
                base.Items.Add(FontsAndColorsHelper.GetNonMarkerItem("urlformat", "urlformat",
                    Color.FromRgb(0, 0, 255),
                    Colors.Black, false));
                base.Items.Add(FontsAndColorsHelper.GetNonMarkerItem("OutputHeading",
                    "OutputHeading",
                    Color.FromRgb(51, 152, 255),
                    Colors.Black, false));
                base.Items.Add(FontsAndColorsHelper.GetNonMarkerItem("OutputError",
                    "OutputError",
                    Color.FromRgb(229, 20, 0),
                    Colors.Black, false));
                base.Items.Add(FontsAndColorsHelper.GetNonMarkerItem("OutputVerbose",
                    "OutputVerbose",
                    Color.FromRgb(113, 113, 113),
                    Colors.Black, false));
                _items = base.Items;
                return _items;
            }
        }

        protected override bool IsLegacyMarker(string item)
        {
            return item == "Current List Location";
        }

        public override string GetCategoryName()
        {
            //TODO Text
            return "Output Window";
        }

        protected override ushort GetPriorityOrder()
        {
            return 4;
        }
    }
}