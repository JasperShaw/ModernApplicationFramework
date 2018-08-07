﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "word wrap glyph")]
    [Name("Visible Whitespace")]
    [UserVisible(true)]
    [Order(After = "Default Priority", Before = "High Priority")]
    internal sealed class WordWrapAndVisibleWhitespaceFormatDefinition : ClassificationFormatDefinition
    {
        public WordWrapAndVisibleWhitespaceFormatDefinition()
        {
            ForegroundColor = Colors.LightGray;
            BackgroundCustomizable = false;
            DisplayName = "Visible Whitespace";
        }
    }
}