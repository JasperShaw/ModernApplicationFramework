﻿using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public class ElementItemsControl : ListBox
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ElementItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ElementItem;
        }

        public ElementItemsControl()
        {
            SelectionMode = SelectionMode.Extended;
        }
    }
}
