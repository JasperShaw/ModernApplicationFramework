using System;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.Controls
{
    public class MenuHostControl : ContentControl
    {
        private bool _contentLoaded;

        public MenuHostControl()
        {
            InitializeComponent();
            DataContext = new MenuHostViewModel(this);
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }

        private void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/MenuHostControl.xaml", UriKind.Relative));
        }
    }
}