using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    public class MenuHostControl : ContentControl
    {
        private bool _contentLoaded;

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu", typeof (Menu), typeof (MenuHostControl), new PropertyMetadata(default(Menu)));

        public Menu Menu
        {
            get { return (Menu) GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        public MenuHostControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/MenuHostControl.xaml", UriKind.Relative));
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }
    }
}
