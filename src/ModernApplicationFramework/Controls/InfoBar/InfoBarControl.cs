using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.InfoBar.Internal;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public class InfoBarControl : UserControl
    {
        private bool _contentLoaded;

        internal InfoBarControl()
        {
            InitializeComponent();
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/InfoBarControl.xaml", UriKind.Relative));
        }
    }
}
