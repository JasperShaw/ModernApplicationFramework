using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class DocumentTabItemWrapper : ContentControl
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutContent), typeof(DocumentTabItemWrapper),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(DocumentTabItemWrapper),
                new FrameworkPropertyMetadata((LayoutItem)null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;


        public LayoutItem LayoutItem => (LayoutItem)GetValue(LayoutItemProperty);

        public LayoutContent Model
        {
            get => (LayoutContent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        static DocumentTabItemWrapper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentTabItemWrapper),
                new FrameworkPropertyMetadata(typeof(DocumentTabItemWrapper)));
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model?.Root.Manager.GetLayoutItemFromModel(Model));
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }


        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DocumentTabItemWrapper)d).OnModelChanged(e);
        }
    }
}