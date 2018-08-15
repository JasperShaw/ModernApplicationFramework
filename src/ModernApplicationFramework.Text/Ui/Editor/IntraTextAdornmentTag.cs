using System;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class IntraTextAdornmentTag : ITag
    {
        public UIElement Adornment { get; }

        public PositionAffinity? Affinity { get; }

        public double? Baseline { get; }

        public double? BottomSpace { get; }

        public AdornmentRemovedCallback RemovalCallback { get; }

        public double? TextHeight { get; }

        public double? TopSpace { get; }

        public IntraTextAdornmentTag(UIElement adornment, AdornmentRemovedCallback removalCallback, double? topSpace,
            double? baseline, double? textHeight, double? bottomSpace, PositionAffinity? affinity)
        {
            Adornment = adornment ?? throw new ArgumentNullException(nameof(adornment));
            RemovalCallback = removalCallback;
            TopSpace = topSpace;
            Baseline = baseline;
            TextHeight = textHeight;
            BottomSpace = bottomSpace;
            Affinity = affinity;
        }

        public IntraTextAdornmentTag(UIElement adornment, AdornmentRemovedCallback removalCallback,
            PositionAffinity? affinity)
            : this(adornment, removalCallback, new double?(), new double?(), new double?(), new double?(), affinity)
        {
        }

        public IntraTextAdornmentTag(UIElement adornment, AdornmentRemovedCallback removalCallback)
            : this(adornment, removalCallback, new double?(), new double?(), new double?(), new double?(),
                new PositionAffinity?())
        {
        }
    }
}