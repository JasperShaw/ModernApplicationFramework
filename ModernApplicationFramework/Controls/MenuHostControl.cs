using System;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Controls
{
    public class MenuHostControl : ContentControl
    {
        private bool _contentLoaded;

        public MenuHostControl()
        {
            InitializeComponent();
            DataContext = IoC.Get<IMenuHostViewModel>();
            MenuHostViewModel.MenuHostControl = this;
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }

        private IMenuHostViewModel MenuHostViewModel => DataContext as IMenuHostViewModel;

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