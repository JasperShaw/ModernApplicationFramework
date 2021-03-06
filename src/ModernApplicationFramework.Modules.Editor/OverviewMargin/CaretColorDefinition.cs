﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("OverviewMarginCaret")]
    [UserVisible(true)]
    [Order(Before = "Default Priority")]
    internal sealed class CaretColorDefinition : EditorFormatDefinition
    {
        public CaretColorDefinition()
        {
            //TODO: Text
            DisplayName = "CaretColorDefinitionName";
            ForegroundColor = Colors.MediumBlue;
            BackgroundCustomizable = false;
        }
    }
}