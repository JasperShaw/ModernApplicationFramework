using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor
{
    internal class DefaultTextNavigator : ITextStructureNavigator
    {
        private readonly ITextBuffer _textBuffer;
        private readonly IContentTypeRegistryService _contentTypeRegistry;

        internal DefaultTextNavigator(ITextBuffer textBuffer, IContentTypeRegistryService contentTypeRegistry)
        {
            _textBuffer = textBuffer;
            _contentTypeRegistry = contentTypeRegistry;
        }

        public TextExtent GetExtentOfWord(SnapshotPoint currentPosition)
        {
            if (currentPosition.Snapshot.TextBuffer != _textBuffer)
                throw new ArgumentException("currentPosition TextBuffer does not match to the current TextBuffer");
            if (currentPosition.Position >= currentPosition.Snapshot.Length - 1)
                return new TextExtent(new SnapshotSpan(currentPosition, currentPosition.Snapshot.Length - currentPosition), true);
            return new TextExtent(new SnapshotSpan(currentPosition, 1), true);
        }

        public SnapshotSpan GetSpanOfEnclosing(SnapshotSpan activeSpan)
        {
            if (activeSpan.IsEmpty && activeSpan.Start != activeSpan.Snapshot.Length)
                return new SnapshotSpan(activeSpan.Start, 1);
            return new SnapshotSpan(activeSpan.Snapshot, 0, activeSpan.Snapshot.Length);
        }

        public SnapshotSpan GetSpanOfFirstChild(SnapshotSpan activeSpan)
        {
            if (activeSpan.IsEmpty)
                return GetSpanOfEnclosing(activeSpan);
            if (activeSpan.Length > 0 && activeSpan.Length < activeSpan.Snapshot.Length)
                return new SnapshotSpan(activeSpan.Snapshot, 0, activeSpan.Snapshot.Length);
            return new SnapshotSpan(activeSpan.Snapshot, 0, 1);
        }

        public SnapshotSpan GetSpanOfNextSibling(SnapshotSpan activeSpan)
        {
            if (activeSpan.IsEmpty)
                return GetSpanOfEnclosing(activeSpan);
            if (activeSpan.End == activeSpan.Snapshot.Length)
                return new SnapshotSpan(activeSpan.Snapshot, 0, activeSpan.Snapshot.Length);
            return new SnapshotSpan(activeSpan.End, 1);
        }

        public SnapshotSpan GetSpanOfPreviousSibling(SnapshotSpan activeSpan)
        {
            if (activeSpan.IsEmpty)
                return GetSpanOfEnclosing(activeSpan);
            if (activeSpan.Start == 0)
                return new SnapshotSpan(activeSpan.Snapshot, 0, activeSpan.Snapshot.Length);
            return new SnapshotSpan(activeSpan.Start - 1, 1);
        }

        public IContentType ContentType => _contentTypeRegistry.UnknownContentType;
    }
}