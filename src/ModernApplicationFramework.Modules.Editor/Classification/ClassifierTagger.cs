using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    internal class ClassifierTagger : IAccurateTagger<ClassificationTag>, IDisposable
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        internal IList<IClassifier> Classifiers { get; }

        internal ClassifierTagger(IList<IClassifier> classifiers)
        {
            Classifiers = classifiers;
            foreach (var classifier in classifiers)
                classifier.ClassificationChanged += OnClassificationChanged;
        }

        public void Dispose()
        {
            foreach (var classifier in Classifiers)
                classifier.ClassificationChanged -= OnClassificationChanged;
            Classifiers.Clear();
            GC.SuppressFinalize(this);
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetAllTags(NormalizedSnapshotSpanCollection spans,
            CancellationToken cancel)
        {
            return from classifier in Classifiers
                let classifier2 = classifier as IAccurateClassifier
                from span in spans
                from classificationSpan in classifier2 != null
                    ? classifier2.GetAllClassificationSpans(span, cancel)
                    : classifier.GetClassificationSpans(span)
                select new TagSpan<ClassificationTag>(classificationSpan.Span,
                    new ClassificationTag(classificationSpan.ClassificationType));
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return from classifier in Classifiers
                from span in spans
                from classificationSpan in classifier.GetClassificationSpans(span)
                select new TagSpan<ClassificationTag>(classificationSpan.Span,
                    new ClassificationTag(classificationSpan.ClassificationType));
        }

        private void OnClassificationChanged(object sender, ClassificationChangedEventArgs e)
        {
            var tagsChanged = TagsChanged;
            tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(e.ChangeSpan));
        }
    }
}