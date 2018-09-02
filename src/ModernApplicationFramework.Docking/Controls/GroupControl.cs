using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Data;

namespace ModernApplicationFramework.Docking.Controls
{
    public class GroupControl : LayoutSynchronizedTabControl
    {
        public static readonly DependencyProperty ContentCornerRadiusProperty =
            DependencyProperty.Register(nameof(ContentCornerRadius), typeof(CornerRadius), typeof(GroupControl),
                new FrameworkPropertyMetadata(new CornerRadius(0.0)));

        public CornerRadius ContentCornerRadius
        {
            get => (CornerRadius)GetValue(ContentCornerRadiusProperty);
            set => SetValue(ContentCornerRadiusProperty, value);
        }

        public GroupControl()
        {
            //Loaded += (param1, param2) => ClearValue(SelectedItemProperty);
            //UtilityMethods.AddPresentationSourceCleanupAction(this, () =>
            //{
            //    BindingOperations.SetBinding(this, SelectedItemProperty, new Binding
            //    {
            //        Mode = BindingMode.OneTime
            //    });
            //    BindingOperations.SetBinding(this, ItemsSourceProperty, new Binding
            //    {
            //        Mode = BindingMode.OneTime
            //    });
            //    DataContext = null;
            //});
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            OnApplyTemplate();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GroupControlTabItem();
        }
    }
}
