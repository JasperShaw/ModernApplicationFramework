using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(ClassificationTag))]
    internal class TextMarkerViewTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            var viewTaggers = GetTextMarkerViewTaggers(buffer);
            if (!viewTaggers.TryGetValue(textView, out var markerViewTagger))
            {
                markerViewTagger = TextMarkerViewTagger.Create(textView, buffer);

                void Closer(object sender, EventArgs e)
                {
                    viewTaggers.Remove(textView);
                    textView.Closed -= Closer;
                }

                viewTaggers.Add(textView, markerViewTagger);
                textView.Closed += Closer;
            }
            return markerViewTagger as ITagger<T>;
        }

        public static IDictionary<ITextView, TextMarkerViewTagger> GetTextMarkerViewTaggers(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new Dictionary<ITextView, TextMarkerViewTagger>());
        }
    }
}