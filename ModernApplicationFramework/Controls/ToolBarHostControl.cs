using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.Controls
{
    public class ToolBarHostControl : ContentControl
    {
        public static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register(
            "DefaultBackground", typeof(Brush), typeof(ToolBarHostControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TopTrayBackgroundProperty = DependencyProperty.Register(
            "TopTrayBackground", typeof(Brush), typeof(ToolBarHostControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        private bool _contentLoaded;

        public ToolBarHostControl()
        {
            InitializeComponent();
            Instance = this;
            DataContext = new ToolBarHostViewModel(this);
        }

        public static ToolBarHostControl Instance { get; private set; }

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

        private ToolBarHostViewModel ToolBarHostViewModel => DataContext as ToolBarHostViewModel;

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


            //var editItem = new ContextMenuItem {Header = "Edit..."};
            //editItem.Click += EditItem_Click;
            //_contextMenu.Items.Add(new Separator());
            //_contextMenu.Items.Add(editItem);
        }

        public override void OnApplyTemplate()
        {
            ToolBarHostViewModel.TopToolBarTay = GetTemplateChild("TopDockTray") as ToolBarTray;
            ToolBarHostViewModel.LeftToolBarTay = GetTemplateChild("LeftDockTray") as ToolBarTray;
            ToolBarHostViewModel.RightToolBarTay = GetTemplateChild("RightDockTray") as ToolBarTray;
            ToolBarHostViewModel.BottomToolBarTay = GetTemplateChild("BottomDockTray") as ToolBarTray;
            base.OnApplyTemplate();
        }
    }
}