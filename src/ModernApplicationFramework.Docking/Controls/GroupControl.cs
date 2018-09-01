using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;

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

    public class LayoutSynchronizedTabControl : TabControl
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            LayoutSynchronizer.Update(this);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            LayoutSynchronizer.Update(this);
        }
    }

    public class GroupControlTabItem : TabItem
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(GroupControlTabItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)new CornerRadius(0.0)));

        public static CornerRadius GetCornerRadius(GroupControlTabItem element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(GroupControlTabItem element, CornerRadius value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(CornerRadiusProperty, value);
        }
    }

    public static class LayoutSynchronizer
    {
        private static int _layoutSynchronizationRefCount;
        private static readonly HashSet<PresentationSource> ElementsToUpdate = new HashSet<PresentationSource>();
        private static bool _isUpdatingLayout;

        public static IDisposable BeginLayoutSynchronization()
        {
            return new LayoutSynchronizationScope();
        }

        public static bool IsSynchronizing => _layoutSynchronizationRefCount > 0;

        public static void Update(Visual element)
        {
            if (!IsSynchronizing || _isUpdatingLayout)
                return;
            var presentationSource = PresentationSource.FromVisual(element);
            if (presentationSource == null)
                return;
            ElementsToUpdate.Add(presentationSource);
        }

        private static void Synchronize()
        {
            if (_isUpdatingLayout)
                return;
            _isUpdatingLayout = true;
            try
            {
                foreach (var presentationSource in ElementsToUpdate)
                    (presentationSource.RootVisual as UIElement)?.UpdateLayout();
                ElementsToUpdate.Clear();
            }
            finally
            {
                _isUpdatingLayout = false;
            }
        }

        private class LayoutSynchronizationScope : DisposableObject
        {
            public LayoutSynchronizationScope()
            {
                ++_layoutSynchronizationRefCount;
            }

            protected override void DisposeManagedResources()
            {
                --_layoutSynchronizationRefCount;
                if (_layoutSynchronizationRefCount != 0)
                    return;
                Synchronize();
            }
        }
    }
}
