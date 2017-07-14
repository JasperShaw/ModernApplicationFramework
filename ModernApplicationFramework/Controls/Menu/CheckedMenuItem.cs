using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Controls.Menu
{
    public class CheckedMenuItem : System.Windows.Controls.MenuItem
    {
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty LinkProperty;

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public object Link
        {
            get => GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        static CheckedMenuItem()
        {
            ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(CheckedMenuItem));
            LinkProperty = DependencyProperty.Register("Link", typeof(object), typeof(CheckedMenuItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckedMenuItem), new FrameworkPropertyMetadata(typeof(CheckedMenuItem)));
        }

        public CheckedMenuItem()
        {
            IsCheckable = true;
            MultiBinding multiBinding = new MultiBinding();
            Collection<BindingBase> bindings1 = multiBinding.Bindings;
            Binding binding1 = new Binding();
            PropertyPath propertyPath1 = new PropertyPath("Value", Array.Empty<object>());
            binding1.Path = propertyPath1;
            binding1.Source = this;
            bindings1.Add(binding1);
            Collection<BindingBase> bindings2 = multiBinding.Bindings;
            Binding binding2 = new Binding();
            PropertyPath propertyPath2 = new PropertyPath("Link", Array.Empty<object>());
            binding2.Path = propertyPath2;
            binding2.Source = this;
            bindings2.Add(binding2);
            multiBinding.Converter = new AreEqualConverter();
            multiBinding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this, IsCheckedProperty, multiBinding);
        }

        protected override void OnClick()
        {
            base.OnClick();
            var binding = BindingOperations.GetBinding(this, LinkProperty);
            if (binding == null || binding.Mode != BindingMode.TwoWay && binding.Mode != BindingMode.Default)
                return;
            Link = Value;
        }
    }
}
