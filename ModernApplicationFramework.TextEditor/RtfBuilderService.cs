using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IRtfBuilderService))]
    internal sealed class RtfBuilderService : IRtfBuilderService2
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMappingService { get; set; }

        [Import]
        internal IClassifierAggregatorService ClassifierAggregatorService { get; set; }

        internal string GenerateRtf(NormalizedSnapshotSpanCollection spans, IClassificationFormatMap formatMap, Color viewBackgroundColor, string delimiter = "\\par ", CancellationToken? cancel = null)
        {
            if (spans.Count == 0)
                return "{\\rtf\\ansi{\\fonttbl}{\\colortbl;}}";
            var classifier = ClassifierAggregatorService.GetClassifier(spans[0].Snapshot.TextBuffer) as IAccurateClassifier;
            var rtf = new RtfBuilder(classifier, formatMap, delimiter, viewBackgroundColor).GenerateRtf(spans, cancel);
            (classifier as IDisposable)?.Dispose();
            return rtf;
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans)
        {
            return GenerateRtf(spans, "\\par ");
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, string delimiter)
        {
            return GenerateRtf(spans, ClassificationFormatMappingService.GetClassificationFormatMap("printer"), Colors.Transparent, delimiter, new CancellationToken?());
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView)
        {
            return GenerateRtf(spans, textView, "\\par ");
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, string delimiter)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            var viewBackgroundColor = (textView).Background is SolidColorBrush background ? background.Color : Colors.White;
            return GenerateRtf(spans, ClassificationFormatMappingService.GetClassificationFormatMap(textView), viewBackgroundColor, delimiter, new CancellationToken?());
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, CancellationToken cancel)
        {
            return GenerateRtf(spans, "\\par ", cancel);
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, string delimiter, CancellationToken cancel)
        {
            return GenerateRtf(spans, ClassificationFormatMappingService.GetClassificationFormatMap("printer"), Colors.Transparent, delimiter, cancel);
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, CancellationToken cancel)
        {
            return GenerateRtf(spans, textView, "\\par ", cancel);
        }

        public string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, string delimiter, CancellationToken cancel)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            var viewBackgroundColor = (textView).Background is SolidColorBrush background ? background.Color : Colors.White;
            return GenerateRtf(spans, ClassificationFormatMappingService.GetClassificationFormatMap(textView), viewBackgroundColor, delimiter, cancel);
        }

        internal class RtfBuilder
        {
            private static readonly XmlLanguage EnUsLanguage = XmlLanguage.GetLanguage("en-US");
            private readonly IList<string> _fontList = new List<string>();
            private readonly IList<Color> _colorList = new List<Color>();
            private readonly IAccurateClassifier _classifier;
            private readonly IClassificationFormatMap _classificationFormatMap;
            private readonly Color _defaultTextColor;
            private readonly Color _defaultBackgroundColor;
            private readonly Color _viewBackgroundColor;
            private readonly string _delimiter;

            internal RtfBuilder(IAccurateClassifier classifier, IClassificationFormatMap classificationFormatMap, string delimiter, Color viewBackgroundColor)
            {
                _classifier = classifier;
                _classificationFormatMap = classificationFormatMap;
                _defaultTextColor = _classificationFormatMap.DefaultTextProperties.ForegroundBrush is SolidColorBrush foregroundBrush ? foregroundBrush.Color : Colors.Black;
                _defaultBackgroundColor = _classificationFormatMap.DefaultTextProperties.BackgroundBrush is SolidColorBrush backgroundBrush ? backgroundBrush.Color : Colors.Transparent;
                _viewBackgroundColor = viewBackgroundColor;
                _delimiter = delimiter;
            }

            public string GenerateRtf(NormalizedSnapshotSpanCollection spans, CancellationToken? cancel = null)
            {
                var body = GenerateBody(spans, cancel);
                var header = GenerateHeader();
                var stringBuilder = new StringBuilder(header.Length + body.Length + 1);
                stringBuilder.Append(header);
                stringBuilder.Append(body);
                stringBuilder.Append("}");
                return stringBuilder.ToString();
            }

            internal string GenerateHeader()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("{\\rtf\\ansi");
                stringBuilder.Append("{\\fonttbl");
                for (var index = 0; index < _fontList.Count; ++index)
                {
                    stringBuilder.Append("{\\f");
                    stringBuilder.Append(index);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(_fontList[index]);
                    stringBuilder.Append(";}");
                }
                stringBuilder.Append("}");
                stringBuilder.Append("{\\colortbl;");
                foreach (var color in _colorList)
                    stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\\red{0}\\green{1}\\blue{2};", color.R, color.G, color.B));
                stringBuilder.Append("}");
                return stringBuilder.ToString();
            }

            internal string GenerateBody(NormalizedSnapshotSpanCollection spans, CancellationToken? cancel)
            {
                var rtfBody = new StringBuilder();
                var fontStyle = FontStyles.Normal;
                var fontWeight = FontWeights.Normal;
                var str = (string)null;
                var num1 = 0;
                var num2 = -1;
                var currentForegroundColor = new Color?();
                var currentBackgroundColor = new Color?();
                for (var index = 0; index < spans.Count; ++index)
                {
                    var span1 = spans[index];
                    foreach (var classificationSpan in GetClassificationSpans(span1, cancel))
                    {
                        var textRunProperties = classificationSpan.ClassificationType == null ? _classificationFormatMap.DefaultTextProperties : (TextRunProperties)_classificationFormatMap.GetTextProperties(classificationSpan.ClassificationType);
                        if (textRunProperties.Typeface.FontFamily.FamilyNames.TryGetValue(EnUsLanguage, out var fontFamily) && fontFamily != null && str != fontFamily)
                        {
                            rtfBody.Append(GetRtfFontKeyword(fontFamily));
                            str = fontFamily;
                        }
                        if (num1 != (int)textRunProperties.FontRenderingEmSize)
                        {
                            num1 = (int)textRunProperties.FontRenderingEmSize;
                            rtfBody.Append("\\fs" + (num1 * 2 * 72 / 96) + " ");
                        }
                        if (fontStyle != textRunProperties.Typeface.Style)
                        {
                            fontStyle = textRunProperties.Typeface.Style;
                            if (fontStyle == FontStyles.Normal)
                                rtfBody.Append("\\i0 ");
                            else if (fontStyle == FontStyles.Italic)
                                rtfBody.Append("\\i ");
                        }
                        if (fontWeight != textRunProperties.Typeface.Weight)
                        {
                            fontWeight = textRunProperties.Typeface.Weight;
                            if (fontWeight == FontWeights.Normal || fontWeight == FontWeights.Regular)
                                rtfBody.Append("\\b0 ");
                            else
                                rtfBody.Append("\\b ");
                        }
                        SetColorProperties(rtfBody, textRunProperties.ForegroundBrush as SolidColorBrush, textRunProperties.BackgroundBrush as SolidColorBrush, ref currentForegroundColor, ref currentBackgroundColor);
                        var numberFromPosition1 = span1.Snapshot.GetLineNumberFromPosition(classificationSpan.Span.Start);
                        var numberFromPosition2 = span1.Snapshot.GetLineNumberFromPosition(classificationSpan.Span.End);
                        for (var lineNumber = numberFromPosition1; lineNumber <= numberFromPosition2; ++lineNumber)
                        {
                            var lineFromLineNumber = span1.Snapshot.GetLineFromLineNumber(lineNumber);
                            var span2 = classificationSpan.Span;
                            var nullable1 = span2.Overlap(lineFromLineNumber.Extent);
                            var nullable2 = nullable1.HasValue ? nullable1.GetValueOrDefault() : new Span?();
                            if (nullable2.HasValue)
                            {
                                var span3 = nullable2.Value;
                                var start = span3.Start;
                                while (true)
                                {
                                    var num3 = start;
                                    span3 = nullable2.Value;
                                    var end = span3.End;
                                    if (num3 < end)
                                    {
                                        rtfBody.Append(GetRtfCharacter(span1.Snapshot[start]));
                                        ++start;
                                    }
                                    else
                                        break;
                                }
                            }
                            span2 = classificationSpan.Span;
                            if (span2.End > lineFromLineNumber.End && num2 != lineNumber)
                            {
                                num2 = lineNumber;
                                rtfBody.Append("\\par ");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(_delimiter) && index + 1 < spans.Count)
                        rtfBody.Append(_delimiter);
                }
                return rtfBody.ToString();
            }

            private IList<NullableClassificationSpan> GetClassificationSpans(SnapshotSpan span, CancellationToken? cancel)
            {
                var classificationSpanList = new List<NullableClassificationSpan>();
                var start = (int)span.Start;
                foreach (var original in cancel.HasValue ? _classifier.GetAllClassificationSpans(span, cancel.Value) : _classifier.GetClassificationSpans(span))
                {
                    if (original.Span.Start > start)
                        classificationSpanList.Add(new NullableClassificationSpan(new SnapshotSpan(span.Snapshot, Span.FromBounds(start, original.Span.Start)), null));
                    classificationSpanList.Add(new NullableClassificationSpan(original));
                    start = original.Span.End;
                }
                if (span.End > start)
                    classificationSpanList.Add(new NullableClassificationSpan(new SnapshotSpan(span.Snapshot, Span.FromBounds(start, span.End)), null));
                return classificationSpanList;
            }

            internal void SetColorProperties(StringBuilder rtfBody, SolidColorBrush foregroundBrush, SolidColorBrush backgroundBrush, ref Color? currentForegroundColor, ref Color? currentBackgroundColor)
            {
                var color1 = foregroundBrush?.Color ?? _defaultTextColor;
                if (!currentForegroundColor.HasValue || color1 != currentForegroundColor.Value)
                {
                    currentForegroundColor = color1;
                    var colorKeywordIndex = GetRtfColorKeywordIndex(color1);
                    rtfBody.Append($"\\cf{colorKeywordIndex} ");
                }
                var color2 = backgroundBrush?.Color ?? _defaultBackgroundColor;
                if (color2.A == 0)
                    color2 = _viewBackgroundColor;
                if (currentBackgroundColor.HasValue && !(color2 != currentBackgroundColor.Value))
                    return;
                currentBackgroundColor = color2;
                var colorKeywordIndex1 = GetRtfColorKeywordIndex(color2);
                rtfBody.Append($"\\cb{colorKeywordIndex1} ");
                rtfBody.Append($"\\highlight{colorKeywordIndex1} ");
            }

            internal static string GetRtfCharacter(char input)
            {
                var stringBuilder = new StringBuilder();
                if (input > '~')
                {
                    stringBuilder.Append("\\uc1\\u");
                    stringBuilder.Append((short)input);
                    stringBuilder.Append("?");
                }
                else
                {
                    if (input == '\\' || input == '{' || input == '}')
                        stringBuilder.Append('\\');
                    stringBuilder.Append(input);
                }
                return stringBuilder.ToString();
            }

            internal string GetRtfFontKeyword(string fontFamily)
            {
                if (!_fontList.Contains(fontFamily))
                    _fontList.Add(fontFamily);
                return "\\f" + _fontList.IndexOf(fontFamily).ToString(CultureInfo.InvariantCulture) + " ";
            }

            internal int GetRtfColorKeywordIndex(Color color)
            {
                if (color.A == 0)
                    return 0;
                if (!_colorList.Contains(color))
                    _colorList.Add(color);
                return _colorList.IndexOf(color) + 1;
            }

            internal class NullableClassificationSpan
            {
                public NullableClassificationSpan(SnapshotSpan span, IClassificationType classificationType)
                {
                    Span = span;
                    ClassificationType = classificationType;
                }

                public NullableClassificationSpan(ClassificationSpan original)
                {
                    Span = original.Span;
                    ClassificationType = original.ClassificationType;
                }

                public SnapshotSpan Span { get; }

                public IClassificationType ClassificationType { get; }
            }
        }
    }
}