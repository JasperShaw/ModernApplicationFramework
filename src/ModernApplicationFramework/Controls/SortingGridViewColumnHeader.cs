using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    public class SortingGridViewColumnHeader : GridViewColumnHeader
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(SortingGridViewColumnHeader), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ListSortDirectionProperty = DependencyProperty.Register(
            "ListSortDirection", typeof(ListSortDirection), typeof(SortingGridViewColumnHeader), new PropertyMetadata(default(ListSortDirection)));


        static SortingGridViewColumnHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SortingGridViewColumnHeader),
                new FrameworkPropertyMetadata(typeof(SortingGridViewColumnHeader)));
        }


        public ListSortDirection ListSortDirection
        {
            get => (ListSortDirection) GetValue(ListSortDirectionProperty);
            set => SetValue(ListSortDirectionProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}