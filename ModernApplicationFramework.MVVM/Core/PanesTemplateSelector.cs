﻿using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    internal class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DocumentTemplate { get; set; }

        public DataTemplate ToolTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ITool)
                return ToolTemplate;

            if (item is IDocument)
                return DocumentTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}