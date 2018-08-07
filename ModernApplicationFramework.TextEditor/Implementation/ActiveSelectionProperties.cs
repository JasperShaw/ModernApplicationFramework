using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("Selected Text")]
    [UserVisible(true)]
    internal sealed class ActiveSelectionProperties : EditorFormatDefinition
    {
        public ActiveSelectionProperties()
        {
            BackgroundColor = SystemColors.HighlightColor;
            DisplayName = "Selected Text";
        }
    }
}