using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class DragUndockHeader : ContentControl, INonClientArea
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutContent), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata((LayoutItem) null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;


        public LayoutItem LayoutItem => (LayoutItem) GetValue(LayoutItemProperty);

        public LayoutContent Model
        {
            get => (LayoutContent) GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }


        public int HitTest(Point point)
        {
            return 0;
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model?.Root.Manager.GetLayoutItemFromModel(Model));
            //UpdateLogicalParent();
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }


        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DragUndockHeader) d).OnModelChanged(e);
        }
    }
}