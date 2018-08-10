using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.TextEditor.Text;
using Action = System.Action;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal sealed class LanguageServiceClassificationTagger : ITagger<IClassificationTag>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly TextDocData _docData;
        private IColorizer _colorizer;
        private readonly IFontsAndColorsInformation _fontsAndColorsInformation;
        private readonly Guid _languageServiceId;
        private bool _inColorization;
        private readonly int _longBufferLineThreshold;
        private bool _isValid;
        private readonly List<int> _cachedStartLineStates;
        private int? _firstChangedLine;
        private int? _lastChangedLine;

        internal LanguageServiceClassificationTagger(ITextBuffer textBuffer, TextDocData docData, Guid languageServiceId, IColorizer colorizer, IFontsAndColorsInformation fontsAndColorsInformation, int longBufferLineThreshold)
        {
            _textBuffer = textBuffer;
            _docData = docData;
            _colorizer = colorizer;
            _languageServiceId = languageServiceId;
            _fontsAndColorsInformation = fontsAndColorsInformation;
            _longBufferLineThreshold = longBufferLineThreshold;
            _isValid = true;
            _cachedStartLineStates = IsStateMaintenanceRequired(false) ? new List<int>() : null;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (IsStateMaintenanceRequired())
            {
                _textBuffer.ChangedLowPriority += textBuffer_Changed;
                _textBuffer.ChangedHighPriority += textBuffer_Changed_HighPriority;
            }
            _docData.OnNewLanguageService += OnLanguageServiceChanged;
        }

        private void UnSubscribeEvents()
        {
            if (IsStateMaintenanceRequired())
            {
                _textBuffer.ChangedLowPriority -= textBuffer_Changed;
                _textBuffer.ChangedHighPriority -= textBuffer_Changed_HighPriority;
            }
            _docData.OnNewLanguageService -= OnLanguageServiceChanged;
        }

        private void OnLanguageServiceChanged(ref Guid newLanguageService)
        {
            if (!(newLanguageService != _languageServiceId))
                return;
            EndColorization();
            _isValid = false;
            UnSubscribeEvents();
            if (_colorizer is IColorizerPrivate colorizer)
                colorizer.CloseColorizer();
            else
                _colorizer.CloseColorizer();
            _colorizer = null;
        }

        internal static bool TryCreateTagger(ITextBuffer textBuffer, IFontsAndColorsInformationService fontAndColorInformation, int longBufferLineThreshold, out LanguageServiceClassificationTagger classifier)
        {
            classifier = null;
            var docData = TextDocDataFromBuffer(textBuffer);
            if (docData == null)
                return false;
            //var guid = typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider).GUID;
            //IntPtr ppvSite;
            //docData.GetSite(ref guid, out ppvSite);
            //Microsoft.VisualStudio.OLE.Interop.IServiceProvider objectForIunknown = Marshal.GetObjectForIUnknown(ppvSite) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            //if (objectForIunknown == null)
            //    return false;
            //Marshal.Release(ppvSite);
            Guid languageServiceId = docData.ActualLanguageServiceID;
            var service =  IoC.GetAll<ILanguageInfo>().FirstOrDefault(x => x.Id == languageServiceId);
            if (service == null || service.GetColorizer(docData, out IColorizer ppColorizer) != 0 || ppColorizer == null)
                return false;
            var colorInformation = fontAndColorInformation.GetFontAndColorInformation(new FontsAndColorsCategory(languageServiceId, CategoryGuids.GuidTextEditorGroup, CategoryGuids.GuidTextEditorGroup));
            classifier = new LanguageServiceClassificationTagger(textBuffer, docData, languageServiceId, ppColorizer, colorInformation, longBufferLineThreshold);
            return true;
        }

        internal bool IsValidForBuffer(ITextBuffer buffer)
        {
            var textDocData = TextDocDataFromBuffer(buffer);
            if (textDocData == null || !_isValid || _textBuffer != buffer)
                return false;
            return textDocData.ActualLanguageServiceID == _languageServiceId;
        }

        internal void textBuffer_Changed_HighPriority(object sender, TextContentChangedEventArgs e)
        {
            if (e.Changes.Count == 0)
                return;
            var count1 = 0;
            foreach (var change in e.Changes)
            {
                count1 += change.LineCountDelta;
                var numberFromPosition1 = e.After.GetLineNumberFromPosition(change.NewPosition);
                var numberFromPosition2 = e.After.GetLineNumberFromPosition(change.NewEnd);
                if (_lastChangedLine.HasValue && _lastChangedLine.Value > numberFromPosition1)
                {
                    var lastChangedLine = _lastChangedLine;
                    var lineCountDelta = change.LineCountDelta;
                    _lastChangedLine = lastChangedLine.HasValue ? lastChangedLine.GetValueOrDefault() + lineCountDelta : new int?();
                }
                _firstChangedLine = _firstChangedLine.HasValue ? Math.Min(_firstChangedLine.Value, numberFromPosition1) : numberFromPosition1;
                _lastChangedLine = _lastChangedLine.HasValue ? Math.Max(_lastChangedLine.Value, numberFromPosition2) : numberFromPosition2;
            }
            var num = _firstChangedLine.Value + 1;
            if (num >= _cachedStartLineStates.Count)
                return;
            if (count1 < 0)
            {
                var count2 = -count1;
                if (_cachedStartLineStates.Count >= num + count2)
                    _cachedStartLineStates.RemoveRange(num, count2);
                else
                    TruncateStartLineStateCache(num);
            }
            else
            {
                if (count1 <= 0)
                    return;
                _cachedStartLineStates.InsertRange(num, Enumerable.Repeat(0, count1));
            }
        }

        internal void textBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            if (e.After != _textBuffer.CurrentSnapshot || !_firstChangedLine.HasValue || !_lastChangedLine.HasValue)
                return;
            var endLine1 = Math.Min(_lastChangedLine.Value, e.After.LineCount - 1);
            var startLine = _firstChangedLine.Value;
            _firstChangedLine = _lastChangedLine = new int?();
            var endLine2 = FixForwardStateCache(startLine, endLine1);
            SendClassificationChangedEvent(e.After, startLine, endLine2);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var source = new List<TagSpan<IClassificationTag>>();
            if (spans == null)
                return source;
            if (!_isValid || _textBuffer.CurrentSnapshot.Length == 0 || spans.Count == 0)
                return source;
            var requestedSnapshot = spans[0].Snapshot;
            foreach (var t in spans)
            {
                var span = t;
                if (requestedSnapshot != _textBuffer.CurrentSnapshot)
                    span = span.TranslateTo(_textBuffer.CurrentSnapshot, SpanTrackingMode.EdgeInclusive);
                BeginColorization();
                var lineForPos = GetLineForPos(span.Start);
                var lineNumber = lineForPos.LineNumber;
                var num = lineForPos.LineNumber;
                if (lineForPos.EndIncludingLineBreak < span.End)
                    num = GetLineNumberForPos(span.End - 1);
                for (var lineNo = lineNumber; lineNo <= num; ++lineNo)
                {
                    var tuple = ClassifyLine(lineNo);
                    if (tuple != null)
                        source.AddRange(tuple.Item1);
                    else
                        break;
                }
                if (requestedSnapshot != _textBuffer.CurrentSnapshot)
                    source = source.Select(s => new TagSpan<IClassificationTag>(s.Span.TranslateTo(requestedSnapshot, SpanTrackingMode.EdgeExclusive), s.Tag)).ToList();
            }
            return source;
        }

        internal Tuple<List<TagSpan<IClassificationTag>>, int> ClassifyLine(int lineNo)
        {
            var lineFromLineNumber = GetLineFromLineNumber(lineNo);
            if (lineFromLineNumber.Length > _longBufferLineThreshold)
                return null;
            var startingStateForLine = TryGetStartingStateForLine(lineNo);
            if (!startingStateForLine.HasValue)
                return null;
            var iState = startingStateForLine.Value;
            var pAttributes = new uint[lineFromLineNumber.Length + 1];
            iState = _colorizer.ColorizeLine(lineNo, lineFromLineNumber.Length, lineFromLineNumber.GetText(), iState,
                pAttributes);
            if (IsStateMaintenanceRequired())
            {
                if (_cachedStartLineStates.Count == lineNo + 1)
                    _cachedStartLineStates.Add(iState);
                else
                    _cachedStartLineStates[lineNo + 1] = iState;
            }
            var tagSpanList = new List<TagSpan<IClassificationTag>>();
            var start = -1;
            var length = -1;
            uint color = 0;
            for (var index = 0; index < pAttributes.Length - 1; ++index)
            {
                var num = pAttributes[index] & byte.MaxValue;
                if (num != 0U)
                {
                    if ((int)num == (int)color)
                    {
                        ++length;
                    }
                    else
                    {
                        if (color != 0U)
                        {
                            IClassificationTag tag = new ClassificationTag(ClassificationTypeFromAttribute(color));
                            tagSpanList.Add(new TagSpan<IClassificationTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, start, length), tag));
                        }
                        start = lineFromLineNumber.Start + index;
                        length = 1;
                    }
                }
                else if (color != 0U)
                {
                    var classificationTag = new ClassificationTag(ClassificationTypeFromAttribute(color));
                    tagSpanList.Add(new TagSpan<IClassificationTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, start, length), classificationTag));
                    start = -1;
                    length = -1;
                }
                color = num;
            }
            if (color != 0U)
            {
                var classificationTag = new ClassificationTag(ClassificationTypeFromAttribute(color));
                tagSpanList.Add(new TagSpan<IClassificationTag>(new SnapshotSpan(_textBuffer.CurrentSnapshot, start, length), classificationTag));
            }
            return Tuple.Create(tagSpanList, iState);
        }

        private int FixForwardStateCache(int startLine, int endLine)
        {
            if (!IsCacheIndexValid(startLine))
                throw new ArgumentOutOfRangeException(nameof(startLine));
            if (!IsCacheIndexValid(endLine))
                throw new ArgumentOutOfRangeException(nameof(endLine));
            var num = startLine + 250;
            var flag = false;
            int lineNo;
            for (lineNo = startLine + 1; lineNo < _cachedStartLineStates.Count && lineNo < num && (lineNo <= endLine + 1 || !flag); ++lineNo)
            {
                var lineFromLineNumber = GetLineFromLineNumber(lineNo - 1);
                if (lineFromLineNumber.Length > _longBufferLineThreshold)
                {
                    --lineNo;
                    break;
                }
                var stateAtEndOfLine = GetStateAtEndOfLine(lineFromLineNumber);
                if (lineNo < _cachedStartLineStates.Count)
                {
                    flag = stateAtEndOfLine == _cachedStartLineStates[lineNo];
                    _cachedStartLineStates[lineNo] = stateAtEndOfLine;
                }
                else
                    break;
            }
            if (lineNo > endLine + 1 && flag)
                return lineNo - 1;
            TruncateStartLineStateCache(lineNo);
            return _textBuffer.CurrentSnapshot.LineCount;
        }

        private bool IsStateMaintenanceRequired(bool useCachedValue = true)
        {
            if (useCachedValue)
                return _cachedStartLineStates != null;
            return _colorizer.GetStateMaintenanceFlag();
        }

        private int TryPopulateStartLineStateCache(int lineNo)
        {
            if (!IsCacheIndexValid(lineNo))
                throw new ArgumentOutOfRangeException(nameof(lineNo));
            if (_cachedStartLineStates.Count == 0)
            {
                var piStartState = _colorizer.GetStartState();
                _cachedStartLineStates.Add(piStartState);
            }
            for (var lineNo1 = _cachedStartLineStates.Count - 1; lineNo1 < lineNo; ++lineNo1)
            {
                var lineFromLineNumber = GetLineFromLineNumber(lineNo1);
                if (lineFromLineNumber.Length > _longBufferLineThreshold)
                    return lineNo1 - 1;
                _cachedStartLineStates.Add(GetStateAtEndOfLine(lineFromLineNumber));
            }
            return lineNo;
        }

        private void TruncateStartLineStateCache(int lineNo)
        {
            if (lineNo >= _cachedStartLineStates.Count)
                return;
            _cachedStartLineStates.RemoveRange(lineNo, _cachedStartLineStates.Count - lineNo);
        }

        private int GetStateAtEndOfLine(ITextSnapshotLine line)
        {
            var cachedStartLineState = _cachedStartLineStates[line.LineNumber];
            return _colorizer.GetStateAtEndOfLine(line.LineNumber, line.Length, line.GetText(), cachedStartLineState);
        }

        internal int? TryGetStartingStateForLine(int lineNo)
        {
            if (!IsCacheIndexValid(lineNo))
                throw new ArgumentOutOfRangeException(nameof(lineNo));
            if (!IsStateMaintenanceRequired())
                return 0;
            if (lineNo >= _cachedStartLineStates.Count && TryPopulateStartLineStateCache(lineNo) < lineNo)
                return new int?();
            return _cachedStartLineStates[lineNo];
        }

        internal void ForceReclassifyLines(int startLine, int endLine)
        {
            if (IsStateMaintenanceRequired() && IsStateMaintenanceRequired(false))
                endLine = FixForwardStateCache(startLine, endLine);
            SendClassificationChangedEvent(_textBuffer.CurrentSnapshot, startLine, endLine);
        }

        internal void BeginColorization()
        {
            if (!_isValid || _inColorization)
                return;
            _colorizer.BeginColorization();
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)EndColorization, DispatcherPriority.Normal, Array.Empty<object>());
            _inColorization = true;
        }

        internal void EndColorization()
        {
            if (!_isValid || !_inColorization)
                return;
            _colorizer?.EndColorization();
            _inColorization = false;
        }

        private void SendClassificationChangedEvent(ITextSnapshot snapshot, int startLine, int endLine)
        {
            int start = snapshot.GetLineFromLineNumber(startLine).Start;
            var end = endLine < snapshot.LineCount ? snapshot.GetLineFromLineNumber(endLine).Start : snapshot.Length;
            var tagsChanged = TagsChanged;
            tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, Span.FromBounds(start, end))));
        }

        private IClassificationType ClassificationTypeFromAttribute(uint color)
        {
            return _fontsAndColorsInformation.GetClassificationType((int)color);
        }

        private static TextDocData TextDocDataFromBuffer(ITextBuffer buffer)
        {
            if (buffer.Properties.TryGetProperty(typeof(IMafTextBuffer), out TextDocData property))
                return property;
            return null;
        }

        private ITextSnapshotLine GetLineForPos(int pos)
        {
            return _textBuffer.CurrentSnapshot.GetLineFromPosition(pos);
        }

        private int GetLineNumberForPos(int pos)
        {
            return _textBuffer.CurrentSnapshot.GetLineNumberFromPosition(pos);
        }

        private ITextSnapshotLine GetLineFromLineNumber(int lineNo)
        {
            return _textBuffer.CurrentSnapshot.GetLineFromLineNumber(lineNo);
        }

        private bool IsCacheIndexValid(int lineNo)
        {
            if (lineNo >= 0)
                return lineNo <= _textBuffer.CurrentSnapshot.LineCount;
            return false;
        }
    }


    public interface IColorizer
    {
        //TODO Implement interface
        bool GetStateMaintenanceFlag();

        int GetStartState();

        int ColorizeLine(int line, int lenght, string text, int state, uint[] attributes);

        int GetStateAtEndOfLine(int line,  int lenght, string text,  int state);

        void CloseColorizer();

        int BeginColorization();

        int EndColorization();
    }

    internal interface IColorizerPrivate : IColorizer
    {

    }

    public interface ILanguageInfo
    {
        //TODO Implement interface
        Guid Id { get; }

        int GetColorizer(IMafTextLines pBuffer, out IColorizer ppColorizer);
    }
}