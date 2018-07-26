﻿using System;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.TextEditor
{
    public class TextFormattingParagraphProperties : TextParagraphProperties
    {
        private readonly TextFormattingRunProperties _defaultTextRunProperties;

        public TextFormattingParagraphProperties(TextFormattingRunProperties defaultTextRunProperties)
        {
            _defaultTextRunProperties = defaultTextRunProperties ?? throw new ArgumentNullException(nameof(defaultTextRunProperties));
            DefaultIncrementalTab = defaultTextRunProperties.FontRenderingEmSize * 4.0;
        }

        public TextFormattingParagraphProperties(TextFormattingRunProperties defaultTextRunProperties, double defaultTabSize)
        {
            if (double.IsNaN(defaultTabSize) || defaultTabSize <= 0.0)
                throw new ArgumentNullException(nameof(defaultTabSize));
            _defaultTextRunProperties = defaultTextRunProperties ?? throw new ArgumentNullException(nameof(defaultTextRunProperties));
            DefaultIncrementalTab = defaultTabSize;
        }

        public override double DefaultIncrementalTab { get; }

        public override TextRunProperties DefaultTextRunProperties => _defaultTextRunProperties;

        public override bool FirstLineInParagraph => false;

        public override FlowDirection FlowDirection => FlowDirection.LeftToRight;

        public override TextAlignment TextAlignment => TextAlignment.Left;

        public override double Indent => 0.0;

        public sealed override double LineHeight => 0.0;

        public override TextMarkerProperties TextMarkerProperties => null;

        public sealed override TextWrapping TextWrapping => TextWrapping.Wrap;
    }
}