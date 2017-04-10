﻿using System.Windows;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls
{
    public class CustomizeControlsListBox : CustomSortListBox, IExposeStyleKeys
    {
        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        public static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<CustomizeControlsListBox>());

        public static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<CustomizeControlsListBox>());

        public static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<CustomizeControlsListBox>());

        public static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<CustomizeControlsListBox>());

        public static ResourceKey SeparatorStyleKey => _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<CustomizeControlsListBox>());

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;


        static CustomizeControlsListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomizeControlsListBox), new FrameworkPropertyMetadata(typeof(CustomizeControlsListBox)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomizeControlsListBoxItem();
        }


        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            StyleUtilities.SelectStyleForItem(element as FrameworkElement, item, this);
        }
    }

    public interface IExposeStyleKeys
    {
        ResourceKey ButtonStyleKey { get; }

        ResourceKey MenuControllerStyleKey { get; }

        ResourceKey ComboBoxStyleKey { get; }

        ResourceKey MenuStyleKey { get; }

        ResourceKey SeparatorStyleKey { get; }
    }
}
