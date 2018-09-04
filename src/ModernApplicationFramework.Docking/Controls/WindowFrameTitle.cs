using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public class WindowFrameTitle : DependencyObject
    {
        private Binding _toolTipBinding;
        private readonly WeakReference _boundFrame = new WeakReference(null);

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(WindowFrameTitle), new PropertyMetadata(default(string)));


        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(nameof(IsDirty),
            typeof(bool), typeof(WindowFrameTitle), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly),
            typeof(bool), typeof(WindowFrameTitle), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register(nameof(ToolTip),
            typeof(string), typeof(WindowFrameTitle), new FrameworkPropertyMetadata(string.Empty));

        public bool IsBound { get; private set; }

        [DefaultValue("")]
        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        [DefaultValue(false)]
        public bool IsDirty
        {
            get => (bool) GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, Boxes.Box(value));
        }

        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, Boxes.Box(value));
        }

        [DefaultValue("")]
        public string ToolTip
        {
            get => (string)GetValue(ToolTipProperty);
            set => SetValue(ToolTipProperty, value);
        }

        public WindowFrameTitle()
        {
            IsBound = false;
        }

        public WindowFrameTitle(LayoutItem layoutItem) : this()
        {
            BindToFrame(layoutItem);
            _boundFrame.Target = layoutItem;
        }

        public bool IsBoundToFrame(LayoutItem frame)
        {
            return _boundFrame.Target == frame;
        }

        private void BindToFrame(LayoutItem layoutItem)
        {
            BindingOperations.SetBinding(this, TitleProperty, new Binding
            {
                Source = layoutItem,
                Path = new PropertyPath(nameof(LayoutItem.Title))
            });
            BindingOperations.SetBinding(this, IsDirtyProperty, new Binding
            {
                Source = layoutItem,
                Path = new PropertyPath(nameof(LayoutItem.IsDirty))
            });
            BindingOperations.SetBinding(this, IsReadOnlyProperty, new Binding
            {
                Source = layoutItem,
                Path = new PropertyPath(nameof(LayoutItem.IsReadonly))
            });
            IsBound = true;
        }

        public void BindToolTip()
        {
            if (_toolTipBinding == null || BindingOperations.GetBinding(this, ToolTipProperty) != null)
                return;
            BindingOperations.SetBinding(this, ToolTipProperty, _toolTipBinding);
        }

        public override string ToString()
        {
            if (Title != null)
                return Title;
            return base.ToString();
        }
    }
}
