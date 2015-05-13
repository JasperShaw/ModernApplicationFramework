using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls.Customize
{
    /// <summary>
    /// Interaktionslogik für ToolBarsPage.xaml
    /// </summary>
    public partial class ToolBarsPage
    {

        private readonly ToolBarHostControl _toolBarHostControl;
        public ToolBarsPage()
        {
            InitializeComponent();
            _toolBarHostControl = ToolBarHostControl.Instance;
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            Loaded += OnLoaded;
            RadioButtonTop.DataContext = System.Windows.Controls.Dock.Top;
            RadioButtonLeft.DataContext = System.Windows.Controls.Dock.Left;
            RadioButtonRight.DataContext = System.Windows.Controls.Dock.Right;
            RadioButtonBottom.DataContext = System.Windows.Controls.Dock.Bottom;
            ToolBarListBox.SelectionChanged += OnItemSelectionChanged;
            RadioButtonTop.Checked += RadioButtonChecked;
            RadioButtonLeft.Checked += RadioButtonChecked;
            RadioButtonRight.Checked += RadioButtonChecked;
            RadioButtonBottom.Checked += RadioButtonChecked;
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null)
                return;
            var toolBar = GetSelectedItem().DataContext as ToolBar;
            if (toolBar == null)
                return;

            var dock = (System.Windows.Controls.Dock) radioButton.DataContext;
            _toolBarHostControl.ChangeToolBarDock(toolBar.IdentifierName, dock);
        }

        private void OnItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var toolBar = GetSelectedItem().DataContext as ToolBar;
            if (toolBar == null)
                return;
            var td = _toolBarHostControl.GetToolBarDock(toolBar.IdentifierName);
            switch (td)
            {
                case System.Windows.Controls.Dock.Top:
                    RadioButtonTop.IsChecked = true;
                    break;
                case System.Windows.Controls.Dock.Left:
                    RadioButtonLeft.IsChecked = true;
                    break;
                case System.Windows.Controls.Dock.Right:
                    RadioButtonRight.IsChecked = true;
                    break;
                default:
                    RadioButtonBottom.IsChecked = true;
                    break;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolBars = _toolBarHostControl.GetToolBars();
            foreach (var toolBar in toolBars)
                ToolBarListBox.Items.Add(CreateItem(toolBar));
            ToolBarListBox.Focus();
            Loaded -= OnLoaded;
        }

        private void OnItemUnchecked(object sender, RoutedEventArgs e)
        {
            var item = sender as CheckableListBoxItem;
            var toolBar = item?.DataContext as ToolBar;
            if (toolBar != null)
                _toolBarHostControl.ChangeToolBarVisibility(toolBar.IdentifierName, false);
        }

        private void OnItemChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = sender as CheckableListBoxItem;
            var toolBar = item?.DataContext as ToolBar;
            if (toolBar != null)
                _toolBarHostControl.ChangeToolBarVisibility(toolBar.IdentifierName, true);
        }

        private CheckableListBoxItem CreateItem(ToolBar toolBar)
        {
            var item = new CheckableListBoxItem
            {
                Content = toolBar.IdentifierName,
                DataContext = toolBar,
                IsChecked = _toolBarHostControl.GetToolBarVisibility(toolBar.IdentifierName),
            };
            item.Checked += OnItemChecked;
            item.Unchecked += OnItemUnchecked;
            return item;
        }

        private CheckableListBoxItem GetSelectedItem()
        {
            var item = ToolBarListBox.SelectedItem as CheckableListBoxItem;
            if (item == null)
                throw new NoNullAllowedException();
            return item;
        }

        public void Connect(int connectionId, object target)
        {
        }
    }
}
