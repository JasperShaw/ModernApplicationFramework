using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal class ProjectionSpanToNormalizedChangeConverter
    {
        private readonly ITextSnapshot _currentSnapshot;
        private readonly ProjectionSpanDiffer _differ;
        private readonly int _textPosition;
        private bool _computed;
        private INormalizedTextChangeCollection _normalizedChanges;

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

        public ProjectionSpanToNormalizedChangeConverter(ProjectionSpanDiffer differ, int textPosition,
            ITextSnapshot currentSnapshot)
        {
            _differ = differ;
            _textPosition = textPosition;
            _currentSnapshot = currentSnapshot;
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

        private void ConstructChanges()
        {
            var differences = _differ.GetDifferences();
            var textChangeList = new List<TextChange>();
            var num = _textPosition;
            foreach (var difference in differences)
            {
                var oldPosition = num + GetMatchSize(_differ.DeletedSpans, difference.Before);
                var textChange = TextChange.Create(oldPosition,
                    BufferFactoryService.StringRebuilderFromSnapshotSpans(_differ.DeletedSpans, difference.Left),
                    BufferFactoryService.StringRebuilderFromSnapshotSpans(_differ.InsertedSpans, difference.Right),
                    _currentSnapshot);
                textChangeList.Add(textChange);
                num = oldPosition + textChange.OldLength;
            }

            _normalizedChanges = NormalizedTextChangeCollection.Create(textChangeList);
        }
    }
}