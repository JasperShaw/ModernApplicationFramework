using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : Conductor<IDockingHostViewModel>, IDockingMainWindowViewModel, IPartImportsSatisfiedNotification
    {
        public ICommand ChangeWindowIconActiveCommand { get; }
        public ICommand ChangeWindowIconPassiveCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand MaximizeResizeCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand SimpleMoveWindowCommand { get; }
        public BitmapImage ActiveIcon { get; set; }
        public bool IsSimpleWindow { get; set; }
        public BitmapImage PassiveIcon { get; set; }
        public bool UseSimpleMovement { get; set; }
        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;
        public Theme Theme { get; set; }
        public MenuHostViewModel MenuHostViewModel { get; set; }
        public StatusBar StatusBar { get; set; }
        public ToolBarHostViewModel ToolBarHostViewModel { get; set; }
        public bool UseStatusBar { get; set; } = true;
        public bool UseTitleBar { get; set; } = true;
        public WindowState WindowState { get; set; }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            ActivateItem(_dockingHost);
        }

#pragma warning disable 649

        [Import]
        private IDockingHostViewModel _dockingHost;

#pragma warning restore 649

        public IDockingHostViewModel DockingHost => _dockingHost;
    }
}
