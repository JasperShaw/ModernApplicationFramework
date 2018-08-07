using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace ModernApplicationFramework.TextEditor.Implementation.OutputClassifier
{
    [Export(typeof(OutputWindowStyleManager))]
    public class OutputWindowStyleManager : IDisposable
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;
        private readonly ITextBuffer _textBuffer;
        private readonly LinkedList<PendingOutput> _pendingOutputs;
        private readonly Dictionary<int, IClassificationType> _lineClassifications;

        public IClassificationType OutputHeadingClassificationType { get; }

        public IClassificationType OutputErrorClassificationType { get; }

        public IClassificationType OutputVerboseClassificationType { get; }

        public OutputWindowStyleManager()
            : this(GetOutputWindowPane())
        {
        }

        public OutputWindowStyleManager(IOutputWindowPane pane)
        {
            _pendingOutputs = new LinkedList<PendingOutput>();
            _lineClassifications = new Dictionary<int, IClassificationType>();
            _textBuffer = GetTextBuffer(pane);
            _textBuffer.Properties?.GetOrCreateSingletonProperty(nameof(OutputWindowStyleManager), () => this);
            _textBuffer.Changed += TextBuffer_Changed;
            OutputHeadingClassificationType = GetClassificationType("OutputHeading");
            OutputErrorClassificationType = GetClassificationType("OutputError");
            OutputVerboseClassificationType = GetClassificationType("OutputVerbose");
        }

        public void Dispose()
        {
            if (_textBuffer == null)
                return;
            _textBuffer.Changed -= TextBuffer_Changed;
            _textBuffer.Properties?.RemoveProperty(nameof(OutputWindowStyleManager));
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetColorizableSpans(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in spans)
            {
                foreach (var overlappedSpan in CalculateOverlappedSpans(span))
                    yield return overlappedSpan;
            }
        }

        public IClassificationType GetClassificationType(string classificationTypeName)
        {
            IClassificationType classificationType = ClassificationRegistry.GetClassificationType(classificationTypeName);
            if (classificationType != null)
                return classificationType;
            throw new InvalidOperationException("Unknown classification type name: " + classificationTypeName);
        }

        private static IOutputWindowPane GetOutputWindowPane()
        {
            IoC.Get<IOutputWindow>().GetPane(out var ppPane);
            return ppPane;
        }

        private IEnumerable<ITagSpan<IClassificationTag>> CalculateOverlappedSpans(SnapshotSpan span)
        {
            var snapshot = span.Snapshot;
            if (snapshot != null && snapshot.Length != 0)
            {
                var lineNumber = span.Start.GetContainingLine().LineNumber;
                var endline = (span.End - 1).GetContainingLine().LineNumber;
                for (var i = lineNumber; i <= endline; ++i)
                {
                    if (_lineClassifications.TryGetValue(i, out var type))
                    {
                        var lineFromLineNumber = snapshot.GetLineFromLineNumber(i);
                        if (lineFromLineNumber != null)
                        {
                            var snapshotSpan = new SnapshotSpan(lineFromLineNumber.Start, lineFromLineNumber.Length);
                            if (!string.IsNullOrEmpty(lineFromLineNumber.Snapshot.GetText(snapshotSpan)))
                                yield return new TagSpan<IClassificationTag>(lineFromLineNumber.Extent, new ClassificationTag(type));
                        }
                    }
                }
            }
        }


        private ITextBuffer GetTextBuffer(IOutputWindowPane pane)
        {
            if (!(pane is IMafUserData vsUserData))
                throw new InvalidOperationException("The shims should allow us to cast to IVsUserData");
            vsUserData.GetData(MafUserDataFormat.TextViewHost, out var pvtData);
            var textBuffer = (pvtData as ITextViewHost)?.TextView?.TextBuffer;
            if (textBuffer != null)
                return textBuffer;
            throw new InvalidOperationException("user data doesn't implement the interface");
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            var linkedListNodeList = new List<LinkedListNode<PendingOutput>>();
            var textChanges = e.Changes.Where(textChange => textChange.Delta > 0);
            var after = e.After;
            foreach (var textChange in textChanges)
            {
                var newText = textChange.NewText;
                for (var linkedListNode = _pendingOutputs.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
                {
                    var pendingOutput = linkedListNode.Value;
                    var length1 = newText.IndexOf(pendingOutput.Message);
                    if (length1 >= 0)
                    {
                        linkedListNodeList.Add(linkedListNode);
                        var stringBuilder = new StringBuilder();
                        stringBuilder.Append(newText.Substring(0, length1));
                        for (var length2 = pendingOutput.Message.Length; length2 > 0; --length2)
                            stringBuilder.Append('\a');
                        stringBuilder.Append(newText.Substring(length1 + pendingOutput.Message.Length));
                        newText = stringBuilder.ToString();
                        var snapshotSpan = new SnapshotSpan(after, textChange.NewSpan.Start + length1, pendingOutput.Message.Length);
                        var snapshotPoint = snapshotSpan.Start;
                        var lineNumber1 = snapshotPoint.GetContainingLine().LineNumber;
                        snapshotPoint = snapshotSpan.End - 1;
                        var lineNumber2 = snapshotPoint.GetContainingLine().LineNumber;
                        for (var key = lineNumber1; key <= lineNumber2; ++key)
                            _lineClassifications.Add(key, pendingOutput.Classification);
                    }
                }
            }
            foreach (var node in linkedListNodeList)
                _pendingOutputs.Remove(node);
            if (e.After.Length != 0 || _lineClassifications.Count == 0)
                return;
            ResetClassifications();
        }

        private void ResetClassifications()
        {
            _pendingOutputs.Clear();
            _lineClassifications.Clear();
        }
    }
}