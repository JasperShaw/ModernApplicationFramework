using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Docking.Layout.Serialization;
using ModernApplicationFramework.Themes;
using ModernApplicationFramework.Themes.LightIDE;
using ModernApplicationFramework.ViewModels;
using Menu = ModernApplicationFramework.Controls.Menu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFrameworkTestAppDock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DockingManager = Manager;
            Icon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
            SourceInitialized += MainWindow_SourceInitialized;
            //((MainWindowViewModel)DataContext).Theme = new LightTheme();
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            ((MainWindowViewModel)DataContext).ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
            ((MainWindowViewModel)DataContext).PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/test.jpg"));

            DeactivatedFloatIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
            ActivatedFloatIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/test.jpg"));

            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test1" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing" }, true, Dock.Left);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing2" }, true, Dock.Left);

            var m = new Menu();

            var mi2 = new MenuItem { Header = "Test2" };
            mi2.Items.Add(new MenuItem { Header = "Test2" });

            var mi = new MenuItem { Header = "Test" };
            mi.Items.Add(mi2);


            m.Items.Add(mi);
            ((MainWindowViewModel)DataContext).MenuHostViewModel.Menu = m;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Application.Current.Resources[ModernApplicationFramework.Core.Themes.EnvironmentColors.MainWindowBackground] = new SolidColorBrush(Colors.Red);
            TextBox.Text = Application.Current.TryFindResource(EnvironmentColors.MainWindowBackground).ToString();
            string s = "";
            foreach (var resource in Application.Current.Resources)
            {
                s += resource.ToString();
            }

        }


        private void DockingManager_OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the document?", "AvalonDock Sample", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void LayoutElement_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var activeContent = ((LayoutRoot)sender).ActiveContent;
            if (e.PropertyName == "ActiveContent")
            {
                Debug.WriteLine("ActiveContent-> {0}", activeContent);
            }
        }

        private void LayoutAnchorable_OnHiding(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to hide this tool?", "AvalonDock", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (((MainWindowViewModel)DataContext).Theme is LightTheme)
                ((MainWindowViewModel)DataContext).Theme = new GenericTheme();
            else
                ((MainWindowViewModel)DataContext).Theme = new LightTheme();

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var documentPane = Manager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            LayoutDocument layoutDocument = new LayoutDocument { Title = "New Document" };

            layoutDocument.Content = new ColorEditor();

            documentPane.Children.Add(layoutDocument);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var serializer = new XmlLayoutSerializer(DockingManager);
            //serializer.LayoutSerializationCallback += (s, args) =>
            //{
            //    args.Content = args.Content;
            //};

            //if (File.Exists(@".\AvalonDock.config"))
            //    serializer.Deserialize(@".\AvalonDock.config");
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            var serializer = new XmlLayoutSerializer(DockingManager);
            serializer.Serialize(@".\AvalonDock.config");
        }
    }
}