using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ModernApplicationFramework.Controls.Internals
{
    [ContentProperty("Child")]
    internal class DummyParentMenuItem : System.Windows.Controls.MenuItem
    {
        public static readonly DependencyProperty ChildProperty;
        private static readonly ControlTemplate ControlTemplate;

        public System.Windows.Controls.MenuItem Child
        {
            get => (System.Windows.Controls.MenuItem)GetValue(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        static DummyParentMenuItem()
        {
            ChildProperty = DependencyProperty.Register("Child", typeof(System.Windows.Controls.MenuItem), typeof(DummyParentMenuItem), new FrameworkPropertyMetadata(ChildPropertyChanged));
            FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            frameworkElementFactory.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(ChildProperty));
            ControlTemplate controlTemplate = new ControlTemplate
            {
                TargetType = typeof(DummyParentMenuItem),
                VisualTree = frameworkElementFactory
            };
            ControlTemplate = controlTemplate;
        }

        public DummyParentMenuItem()
        {
            Template = ControlTemplate;
            Focusable = false;
            ((INotifyCollectionChanged) Items).CollectionChanged += ItemsCollectionChanged;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ItemsSourceProperty && ItemsSource != null)
            {
                ItemsSource = null;
                throw new InvalidOperationException("DummyParentMenuItem cannot have an ItemsSource");
            }
        }


        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Items.Count == 0)
            {
                if (Child == null)
                    return;
                Child = null;
            }
            else
            {
                System.Windows.Controls.MenuItem menuItem = Items[0] as System.Windows.Controls.MenuItem;
                if (Items.Count == 1 && menuItem != null)
                {
                    if (Equals(Child, menuItem))
                        return;
                    Child = menuItem;
                }
                else
                {
                    Items.Clear();
                    throw new InvalidOperationException("DummyParentMenuItem can have at most one child item, and it must be of type MenuItem");
                }
            }
        }

        private static void ChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dummyParentMenuItem = (DummyParentMenuItem)d;


            ((INotifyCollectionChanged) dummyParentMenuItem.Items).CollectionChanged -= dummyParentMenuItem.ItemsCollectionChanged;
            try
            {
                dummyParentMenuItem.Items.Clear();
                System.Windows.Controls.MenuItem child = dummyParentMenuItem.Child;
                if (child == null)
                    return;
                dummyParentMenuItem.Items.Add(child);
            }
            finally
            {
                ((INotifyCollectionChanged) dummyParentMenuItem.Items).CollectionChanged += dummyParentMenuItem.ItemsCollectionChanged;
            }
        }
    }
}
