using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Text.Differencing;

namespace ModernApplicationFramework.TextEditor
{
    internal class ProjectionSpanToNormalizedChangeConverter
    {
        private INormalizedTextChangeCollection _normalizedChanges;
        private bool _computed;
        private readonly int _textPosition;
        private readonly ProjectionSpanDiffer _differ;
        private readonly ITextSnapshot _currentSnapshot;

        public ProjectionSpanToNormalizedChangeConverter(ProjectionSpanDiffer differ, int textPosition, ITextSnapshot currentSnapshot)
        {
            _differ = differ;
            _textPosition = textPosition;
            _currentSnapshot = currentSnapshot;
        }

        public INormalizedTextChangeCollection NormalizedChanges
        {
            get
            {
                if (_computed)
                    return _normalizedChanges;
                ConstructChanges();
                _computed = true;
                return _normalizedChanges;
            }
        }

        private void ConstructChanges()
        {
            var differences = _differ.GetDifferences();
            var textChangeList = new List<TextChange>();
            var num = _textPosition;
            foreach (var difference in differences)
            {
                var oldPosition = num + GetMatchSize(_differ.DeletedSpans, difference.Before);
                var textChange = TextChange.Create(oldPosition, BufferFactoryService.StringRebuilderFromSnapshotSpans(_differ.DeletedSpans, difference.Left), BufferFactoryService.StringRebuilderFromSnapshotSpans(_differ.InsertedSpans, difference.Right), _currentSnapshot);
                textChangeList.Add(textChange);
                num = oldPosition + textChange.OldLength;
            }
            _normalizedChanges = NormalizedTextChangeCollection.Create(textChangeList);
        }

        private static int GetMatchSize(ReadOnlyCollection<SnapshotSpan> spans, Match match)
        {
            var num = 0;
            if (match != null)
            {
                var left = match.Left;
                for (var start = left.Start; start < left.End; ++start)
                    num += spans[start].Length;
            }
            return num;
        }
    }
}