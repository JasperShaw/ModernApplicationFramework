using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernApplicationFramework.Controls
{
    public class LiveTextBlock : TextBlock
    {
        public static readonly DependencyProperty LiveTextProperty = DependencyProperty.Register(
            "LiveText", typeof(string), typeof(LiveTextBlock), new PropertyMetadata(default(string)));

        public string LiveText
        {
            get => (string) GetValue(LiveTextProperty);
            set => SetValue(LiveTextProperty, value);
        }

        public LiveTextBlock()
        {
            var binding = new Binding
            {
                Path = new PropertyPath(TextProperty),
                Source = this,
                Mode = BindingMode.OneWay
            };
            SetBinding(LiveTextProperty, binding);
        }
    }
}
