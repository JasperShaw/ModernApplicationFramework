using System.Windows;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    /// <inheritdoc cref="CustomSortListBox" />
    /// <summary>
    /// A custom list box control that visualizes each item based on its data model differently
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.ListBoxes.CustomSortListBox" />
    /// <seealso cref="T:ModernApplicationFramework.Controls.ListBoxes.IExposeStyleKeys" />
    public class CustomizeControlsListBox : CustomSortListBox, IExposeStyleKeys
    {
        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        /// <summary>
        /// The style key for a simple button
        /// </summary>
        public static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<CustomizeControlsListBox>());

        /// <summary>
        /// The style key for a menu controller
        /// </summary>
        public static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<CustomizeControlsListBox>());

        /// <summary>
        /// The style key for a combo box
        /// </summary>
        public static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<CustomizeControlsListBox>());

        /// <summary>
        /// The style key for a menu item
        /// </summary>
        public static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<CustomizeControlsListBox>());

        /// <summary>
        /// The style key for a separator
        /// </summary>
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
}
