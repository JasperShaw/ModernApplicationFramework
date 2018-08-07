﻿using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public struct AllColorableItemInfo
    {
        public ColorableItemInfo Info;
        public string Name;
        public string LocalizedName;
        public Color AutoForeground;
        public Color AutoBackground;
        public uint MarkerVisualStyle;
        public Linestyle LineStyle;
        public FcItemflags Flags;
        public string Description;
        public bool NameValid;
        public bool LocalizedNameValid;
        public bool AutoForegroundValid;
        public bool AutoBackgroundValid;
        public bool MarkerVisualStyleValid;
        public bool LineStyleValid;
        public bool FlagsValid;
        public bool DescriptionValid;
    }
}