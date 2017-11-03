using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class ToolBarHostControl : ContentControl
    {
        public static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register(
            "DefaultBackground", typeof(Brush), typeof(ToolBarHostControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TopTrayBackgroundProperty = DependencyProperty.Register(
            "TopTrayBackground", typeof(Brush), typeof(ToolBarHostControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        private bool _contentLoaded;
        private InfoBarHostControl _infoBarHost;


        public InfoBarHostControl InfoBarHost
        {
            get
            {
                if (_infoBarHost == null)
                    ApplyTemplate();
                return _infoBarHost;
            }
        }


        public ToolBarHostControl()
        {
            InitializeComponent();
            DataContext = IoC.Get<IToolBarHostViewModel>();
        }

        public Brush DefaultBackground
        {
            get => (Brush) GetValue(DefaultBackgroundProperty);
            set => SetValue(DefaultBackgroundProperty, value);
        }

        public Brush TopTrayBackground
        {
            get => (Brush) GetValue(TopTrayBackgroundProperty);
            set => SetValue(TopTrayBackgroundProperty, value);
        }

        private IToolBarHostViewModel ToolBarHostViewModel => DataContext as IToolBarHostViewModel;

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/ToolbarHostControl.xaml", UriKind.Relative));
        }

        public override void OnApplyTemplate()
        {
            ToolBarHostViewModel.TopToolBarTray = GetTemplateChild("TopDockTray") as ToolBarTray;
            ToolBarHostViewModel.LeftToolBarTray = GetTemplateChild("LeftDockTray") as ToolBarTray;
            ToolBarHostViewModel.RightToolBarTray = GetTemplateChild("RightDockTray") as ToolBarTray;
            ToolBarHostViewModel.BottomToolBarTray = GetTemplateChild("BottomDockTray") as ToolBarTray;
            ToolBarHostViewModel.Build();
            base.OnApplyTemplate();
            _infoBarHost = GetTemplateChild("PART_InfoBarHost") as InfoBarHostControl;
        }
    }
}