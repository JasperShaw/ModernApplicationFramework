﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("OverviewMarginCollapsedRegion")]
    [UserVisible(true)]
    [Order(Before = "Default Priority")]
    internal sealed class ElisionColorDefinition : EditorFormatDefinition
    {
        public ElisionColorDefinition()
        {
            //TODO: Text
            DisplayName = "ElisionColorDefinitionName";
            ForegroundCustomizable = false;
            BackgroundColor = Color.FromRgb(155, 165, 185);
        }
    }
}