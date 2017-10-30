using System;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class MenuHostControl : ContentControl
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

        private MenuHostViewModel MenuHostViewModel => DataContext as MenuHostViewModel;

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