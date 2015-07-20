using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    [ContentProperty("Text")]
    public class OutlineTextBlock : FrameworkElement
    {
        public static readonly DependencyProperty BoldProperty = DependencyProperty.Register(
            "Bold",
            typeof (bool),
            typeof (OutlineTextBlock),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnOutlineTextInvalidated,
                null
                )
            );

        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof (OutlineTextBlock),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(
                typeof (OutlineTextBlock),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof (OutlineTextBlock),
                new FrameworkPropertyMetadata(SystemColors.ControlTextBrush,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ItalicProperty = DependencyProperty.Register(
            "Italic",
            typeof (bool),
            typeof (OutlineTextBlock),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnOutlineTextInvalidated,
                null
                )
            );

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke",
            typeof (Brush),
            typeof (OutlineTextBlock),
            new FrameworkPropertyMetadata(
                new SolidColorBrush(Colors.Teal),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnOutlineTextInvalidated,
                null
                )
            );

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness",
            typeof (ushort),
            typeof (OutlineTextBlock),
            new FrameworkPropertyMetadata(
                (ushort) 0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnOutlineTextInvalidated,
                null
                )
            );

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof (string),
                typeof (OutlineTextBlock),
                new FrameworkPropertyMetadata(
                    String.Empty,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnOutlineTextInvalidated));

        private Geometry _textGeometry;

        public bool Bold
        {
            get { return (bool) GetValue(BoldProperty); }

            set { SetValue(BoldProperty, value); }
        }

        [Bindable(true), Category("Appearance")]
        [Localizability(LocalizationCategory.Font)]
        public FontFamily FontFamily
        {
            get { return (FontFamily) GetValue(FontFamilyProperty); }

            set { SetValue(FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double) GetValue(FontSizeProperty); }

            set { SetValue(FontSizeProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush) GetValue(ForegroundProperty); }

            set { SetValue(ForegroundProperty, value); }
        }

        public bool Italic
        {
            get { return (bool) GetValue(ItalicProperty); }

            set { SetValue(ItalicProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }

            set { SetValue(StrokeProperty, value); }
        }

        public ushort StrokeThickness
        {
            get { return (ushort) GetValue(StrokeThicknessProperty); }

            set { SetValue(StrokeThicknessProperty, value); }
        }

        [Bindable(true), Category("Appearance")]
        public string Text
        {
            get { return (string) GetValue(TextProperty); }

            set { SetValue(TextProperty, value); }
        }

        public void CreateText()
        {
            FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;

            if (Bold)
                fontWeight = FontWeights.Bold;
            if (Italic)
                fontStyle = FontStyles.Italic;

            var formattedText = new FormattedText(
                Text,
                CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name),
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, fontStyle, fontWeight, FontStretches.Normal),
                FontSize,
                Brushes.Black // This brush does not matter since we use the geometry of the text. 
                );

            _textGeometry = formattedText.BuildGeometry(new Point(0, 0));
            MinWidth = formattedText.Width;
            MinHeight = formattedText.Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            CreateText();
            drawingContext.DrawGeometry(Foreground, new Pen(Stroke, StrokeThickness), _textGeometry);
        }

        private static void OnOutlineTextInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OutlineTextBlock) d).CreateText();
        }
    }
}