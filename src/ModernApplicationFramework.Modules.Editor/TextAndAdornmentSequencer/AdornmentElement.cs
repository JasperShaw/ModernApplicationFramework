﻿using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.TextAndAdornmentSequencer
{
    internal class AdornmentElement : IAdornmentElement
    {
        public readonly IMappingSpan MappingSpan;
        public readonly IMappingTagSpan<SpaceNegotiatingAdornmentTag> MappingTag;
        public readonly SnapshotSpan SourceLocation;

        public PositionAffinity Affinity => MappingTag.Tag.Affinity;

        public double Baseline => MappingTag.Tag.Baseline;

        public double BottomSpace => MappingTag.Tag.BottomSpace;

        public object IdentityTag => MappingTag.Tag.IdentityTag;

        public object ProviderTag => MappingTag.Tag.ProviderTag;

        public bool ShouldRenderText => false;

        public IMappingSpan Span => MappingSpan;

        public double TextHeight => MappingTag.Tag.TextHeight;

        public double TopSpace => MappingTag.Tag.TopSpace;

        public double Width => MappingTag.Tag.Width;

        private AdornmentElement(NormalizedSnapshotSpanCollection tagSourceSpans,
            IMappingTagSpan<SpaceNegotiatingAdornmentTag> tag)
        {
            MappingTag = tag;
            SourceLocation = tagSourceSpans[0];
            MappingSpan = tag.Span;
        }

        public static AdornmentElement Create(ITextSnapshot topSnapshot, ITextSnapshot sourceSnapshot,
            IMappingTagSpan<SpaceNegotiatingAdornmentTag> tag, NormalizedSnapshotSpanCollection lineSpans)
        {
            var spans1 = tag.Span.GetSpans(topSnapshot);
            if (spans1.Count > 0)
            {
                var source = spans1;

                bool Func(SnapshotSpan s)
                {
                    return s.Length > 0;
                }

                if (source.Any(Func))
                    return null;
            }

            var spans2 = tag.Span.GetSpans(sourceSnapshot);
            if (spans2.Count == 1 && lineSpans.IntersectsWith(spans2))
                return new AdornmentElement(spans2, tag);
            return null;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "at {0} adornment {1}", SourceLocation,
                MappingTag.Tag.IdentityTag);
        }

        internal static int Compare(AdornmentElement left, AdornmentElement right)
        {
            var start = left.SourceLocation.Start;
            var position1 = start.Position;
            start = right.SourceLocation.Start;
            var position2 = start.Position;
            var num1 = position1 - position2;
            if (num1 != 0)
                return num1;
            var num2 = left.SourceLocation.Length - right.SourceLocation.Length;
            if (num2 != 0)
                return num2;
            return left.Affinity.CompareTo(right.Affinity);
        }

        internal AdornmentElement Grow(SnapshotPoint newEnd)
        {
            return SetSourceSpan(SourceLocation.Start, newEnd, MappingTag.Tag.Affinity);
        }

        internal AdornmentElement Shrink(SnapshotPoint newEnd)
        {
            return SetSourceSpan(newEnd, newEnd, PositionAffinity.Predecessor);
        }

        private AdornmentElement SetSourceSpan(SnapshotPoint start, SnapshotPoint end,
            PositionAffinity newPositionAffinity)
        {
            var tag = MappingTag.Tag;
            var span = new SnapshotSpan(start, end);
            return new AdornmentElement(new NormalizedSnapshotSpanCollection(span),
                new MappingTagSpan<SpaceNegotiatingAdornmentTag>(
                    MappingSpan.BufferGraph.CreateMappingSpan(span, SpanTrackingMode.EdgeExclusive),
                    new SpaceNegotiatingAdornmentTag(tag.Width, tag.TopSpace, tag.Baseline, tag.TextHeight,
                        tag.BottomSpace, newPositionAffinity, tag.IdentityTag, tag.ProviderTag)));
        }
    }
}