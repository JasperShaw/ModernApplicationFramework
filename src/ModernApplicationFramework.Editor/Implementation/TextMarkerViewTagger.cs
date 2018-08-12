using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class TextMarkerViewTagger : ITagger<ClassificationTag>
    {
        protected ITextView View;
        protected ITextBuffer Buffer;
        protected MarkerManager Manager;
        protected ViewMarkerTypeManager _viewManager;

        public TextMarkerViewTagger(ITextView view, ITextBuffer buffer, MarkerManager manager)
        {
            View = view;
            Buffer = buffer;
            Manager = manager;
        }

        public static TextMarkerViewTagger Create(ITextView view, ITextBuffer buffer)
        {
            buffer.Properties.TryGetProperty<MarkerManager>(typeof(MarkerManager), out var property1);
            if (property1 != null)
                return new TextMarkerViewTagger(view, buffer, property1);
            if (buffer.Properties.TryGetProperty<TextDocData>(typeof(Implementation.IMafTextBuffer), out var property2) && property2._dataTextBuffer == buffer && property2._documentTextBuffer != buffer && property2.MarkerManager != null)
                return new SurfaceTextMarkerViewTagger(view, buffer, property2.MarkerManager);
            return null;
        }

        public ViewMarkerTypeManager ViewManager
        {
            get
            {
                if (_viewManager == null)
                    View.Properties.TryGetProperty(typeof(ViewMarkerTypeManager), out _viewManager);
                return _viewManager;
            }
        }

        public void RaiseChangedEvent(SnapshotSpan span)
        {
            var tagsChanged = TagsChanged;
            if (tagsChanged == null || ViewManager == null)
                return;
            tagsChanged(this, new SnapshotSpanEventArgs(span));
        }

        public virtual IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (ViewManager == null)
                yield break;
            foreach (var span in spans)
            {
                foreach (var classificationTag in Manager.GetClassificationTags(span, ViewManager))
                    yield return classificationTag;
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}