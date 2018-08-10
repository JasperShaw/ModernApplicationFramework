using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Adornments;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextMarkerProviderFactory))]
    [Export(typeof(ITaggerProvider))]
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole("ANALYZABLE")]
    [TextViewRole("INTERACTIVE")]
    [TextViewRole("ENHANCED_SCROLLBAR_PREVIEW")]
    [TagType(typeof(TextMarkerTag))]
    internal sealed class TextMarkerProviderFactory : ITextMarkerProviderFactory, ITaggerProvider, ITextViewCreationListener
    {
        [Export]
        [Name("TextMarker")]
        [Order(After = "Outlining", Before = "SelectionAndProvisionHighlight")]
        internal AdornmentLayerDefinition textMarkerLayer;
        public const string NegativeTextMarkerLayer = "negativetextmarkerlayer";
        [Export]
        [Name("negativetextmarkerlayer")]
        [Order(After = "Outlining", Before = "VsTextMarker")]
        [Order(Before = "TextMarker")]
        internal AdornmentLayerDefinition negativeTextMarkerLayer;

        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; set; }

        public void TextViewCreated(ITextView textView)
        {
            TextMarkerVisualManager markerVisualManager = new TextMarkerVisualManager(textView, TagAggregatorFactoryService, EditorFormatMapService);
        }

        public SimpleTagger<TextMarkerTag> GetTextMarkerTagger(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            return CreateTextMarkerTaggerInternal(textBuffer);
        }

        internal static SimpleTagger<TextMarkerTag> CreateTextMarkerTaggerInternal(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty<SimpleTagger<TextMarkerTag>>(() => new SimpleTagger<TextMarkerTag>(textBuffer));
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            return CreateTextMarkerTaggerInternal(buffer) as ITagger<T>;
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("bookmark")]
        internal sealed class BookmarkMarkerDefinition : MarkerFormatDefinition
        {
            public BookmarkMarkerDefinition()
            {
                ZOrder = 1;
                var num = 32;
                var lightBlue = Colors.LightBlue;
                int r = lightBlue.R;
                lightBlue = Colors.LightBlue;
                int g = lightBlue.G;
                lightBlue = Colors.LightBlue;
                int b = lightBlue.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("breakpoint")]
        internal sealed class BreakpointMarkerDefinition : MarkerFormatDefinition
        {
            public BreakpointMarkerDefinition()
            {
                ZOrder = 2;
                var num = 96;
                var red = Colors.Red;
                int r = red.R;
                red = Colors.Red;
                int g = red.G;
                red = Colors.Red;
                int b = red.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("currentstatement")]
        internal sealed class CurrentStatementMarkerDefinition : MarkerFormatDefinition
        {
            public CurrentStatementMarkerDefinition()
            {
                ZOrder = 3;
                var num = 96;
                var yellow = Colors.Yellow;
                int r = yellow.R;
                yellow = Colors.Yellow;
                int g = yellow.G;
                yellow = Colors.Yellow;
                int b = yellow.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("returnstatement")]
        internal sealed class ReturnStatementMarkerDefinition : MarkerFormatDefinition
        {
            public ReturnStatementMarkerDefinition()
            {
                ZOrder = 4;
                var num = 96;
                var green = Colors.Green;
                int r = green.R;
                green = Colors.Green;
                int g = green.G;
                green = Colors.Green;
                int b = green.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("stepbackcurrentstatement")]
        internal sealed class StepBackCurrentStatementMarkerDefinition : MarkerFormatDefinition
        {
            public StepBackCurrentStatementMarkerDefinition()
            {
                ZOrder = 5;
                var num = 32;
                var purple = Colors.Purple;
                int r = purple.R;
                purple = Colors.Purple;
                int g = purple.G;
                purple = Colors.Purple;
                int b = purple.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("vivid")]
        internal sealed class VividMarkerDefinition : MarkerFormatDefinition
        {
            public VividMarkerDefinition()
            {
                ZOrder = 6;
                var num = 128;
                var green = Colors.Green;
                int r = green.R;
                green = Colors.Green;
                int g = green.G;
                green = Colors.Green;
                int b = green.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("blue")]
        internal sealed class BlueMarkerDefinition : MarkerFormatDefinition
        {
            public BlueMarkerDefinition()
            {
                ZOrder = 7;
                var num = 32;
                var blue = Colors.Blue;
                int r = blue.R;
                blue = Colors.Blue;
                int g = blue.G;
                blue = Colors.Blue;
                int b = blue.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Border = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("remove line")]
        internal sealed class RemoveLineMarkerDefinition : MarkerFormatDefinition
        {
            public RemoveLineMarkerDefinition()
            {
                ZOrder = 7;
                Fill = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>(3)
                {
                    new GradientStop(Color.FromArgb(127, byte.MaxValue, 0, 0), 0.0),
                    new GradientStop(Color.FromArgb(127, byte.MaxValue, 119, 119), 0.488584),
                    new GradientStop(Color.FromArgb(127, byte.MaxValue, 0, 0), 0.949772)
                }))
                {
                    StartPoint = new Point(0.5, -0.814516),
                    EndPoint = new Point(0.5, 1.81452)
                };
                Border = null;
                Fill.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("add line")]
        internal sealed class AddLineMarkerDefinition : MarkerFormatDefinition
        {
            public AddLineMarkerDefinition()
            {
                ZOrder = 8;
                Fill = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>(3)
                {
                    new GradientStop(Color.FromArgb(128, byte.MaxValue, byte.MaxValue, 0), 0.0),
                    new GradientStop(Color.FromArgb(128, byte.MaxValue, byte.MaxValue, 115), 0.488584),
                    new GradientStop(Color.FromArgb(128, byte.MaxValue, byte.MaxValue, 0), 0.949772)
                }))
                {
                    StartPoint = new Point(0.5, -0.814516),
                    EndPoint = new Point(0.5, 1.81452)
                };
                Border = null;
                Fill.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("remove word")]
        internal sealed class RemoveWordMarkerDefinition : MarkerFormatDefinition
        {
            public RemoveWordMarkerDefinition()
            {
                ZOrder = 9;
                Fill = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>(3)
                {
                    new GradientStop(Color.FromArgb(143, byte.MaxValue, 0, 0), 0.0),
                    new GradientStop(Color.FromArgb(143, byte.MaxValue, 119, 96), 0.488584),
                    new GradientStop(Color.FromArgb(143, byte.MaxValue, 0, 0), 0.949772)
                }))
                {
                    StartPoint = new Point(0.5, -0.814516),
                    EndPoint = new Point(0.5, 1.81452)
                };
                Border = new Pen(new SolidColorBrush(Colors.DarkRed), 0.5);
                Border.Freeze();
                Fill.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("add word")]
        internal sealed class AddWordMarkerDefinition : MarkerFormatDefinition
        {
            public AddWordMarkerDefinition()
            {
                ZOrder = 10;
                Fill = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>(3)
                {
                    new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0), 0.0),
                    new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, 96), 0.488584),
                    new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0), 0.949772)
                }))
                {
                    StartPoint = new Point(0.5, -0.814516),
                    EndPoint = new Point(0.5, 1.81452)
                };
                Border = new Pen(new SolidColorBrush(Colors.Orange), 0.5);
                Fill.Freeze();
                Border.Freeze();
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("bracehighlight")]
        internal sealed class BraceHighlightMarkerDefinition : MarkerFormatDefinition
        {
            public BraceHighlightMarkerDefinition()
            {
                ZOrder = 11;
                var num = 32;
                var gray = Colors.Gray;
                int r = gray.R;
                gray = Colors.Gray;
                int g = gray.G;
                gray = Colors.Gray;
                int b = gray.B;
                Fill = new SolidColorBrush(Color.FromArgb((byte)num, (byte)r, (byte)g, (byte)b));
                Fill.Freeze();
            }
        }
    }
}