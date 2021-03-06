﻿using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public class SpaceNegotiatingAdornmentTag : ITag
    {
        public PositionAffinity Affinity { get; }

        public double Baseline { get; }

        public double BottomSpace { get; }

        public object IdentityTag { get; }

        public object ProviderTag { get; }

        public double TextHeight { get; }

        public double TopSpace { get; }
        public double Width { get; }

        public SpaceNegotiatingAdornmentTag(double width, double topSpace, double baseline, double textHeight,
            double bottomSpace, PositionAffinity affinity, object identityTag, object providerTag)
        {
            Width = width;
            TopSpace = topSpace;
            Baseline = baseline;
            TextHeight = textHeight;
            BottomSpace = bottomSpace;
            Affinity = affinity;
            IdentityTag = identityTag;
            ProviderTag = providerTag;
        }
    }
}