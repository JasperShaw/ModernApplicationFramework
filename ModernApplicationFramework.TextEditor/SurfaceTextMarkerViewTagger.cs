using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    internal class SurfaceTextMarkerViewTagger : TextMarkerViewTagger
    {
        private int _state;

        public SurfaceTextMarkerViewTagger(ITextView view, ITextBuffer buffer, MarkerManager manager)
            : base(view, buffer, manager)
        {
        }

        public override IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            ITextSnapshot sourceSnapshot = spans[0].Snapshot;
            ITextSnapshot targetSnapshot = FindCorrespondingSnapshot(sourceSnapshot, Manager.Buffer);
            foreach (SnapshotSpan span in spans)
            {
                SnapshotSpan translatedSpan = new SnapshotSpan(targetSnapshot, span.Span);
                foreach (ITagSpan<ClassificationTag> classificationTag in Manager.GetClassificationTags(translatedSpan, ViewManager))
                    yield return new TagSpan<ClassificationTag>(new SnapshotSpan(sourceSnapshot, classificationTag.Span.Span), classificationTag.Tag);
            }
        }

        internal static ITextSnapshot FindCorrespondingSnapshot(ITextSnapshot sourceSnapshot, ITextBuffer targetBuffer)
        {
            if (sourceSnapshot.TextBuffer == targetBuffer)
                return sourceSnapshot;
            return (sourceSnapshot as IProjectionSnapshot)?.GetMatchingSnapshotInClosure(targetBuffer);
        }
    }
}