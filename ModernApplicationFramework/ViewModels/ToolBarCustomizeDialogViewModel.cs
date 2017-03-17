﻿using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Customize;
using ModernApplicationFramework.Interfaces.ViewModels;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.ViewModels
{
    internal sealed class ToolBarCustomizeDialogViewModel : ViewModelBase
    {
        private IToolBarHostViewModel _toolBarHostViewModel;

        internal ToolBarCustomizeDialogViewModel(ToolBarsPage toolBarsPage)
        {
            ToolBarsPage = toolBarsPage;
            ToolBarsPage.Loaded += ToolBarsPage_Loaded;

            ToolBarsPage.ToolBarListBox.SelectionChanged += OnItemSelectionChanged;

            SetupRadioButtons();
        }

        public ToolBarsPage ToolBarsPage { get; }

        public IToolBarHostViewModel ToolBarHostViewModel
        {
            get => _toolBarHostViewModel;
            set
            {
                _toolBarHostViewModel = value;
                OnPropertyChanged();
            }
        }

        private CheckableListBoxItem CreateItem(ToolBar toolBar)
        {
            //var item = new CheckableListBoxItem
            //{
            //    Content = toolBar.IdentifierName,
            //    DataContext = toolBar,
            //    IsChecked = ToolBarHostViewModel.GetToolBarVisibility(toolBar.IdentifierName)
            //};
            //item.Checked += OnItemChecked;
            //item.Unchecked += OnItemUnchecked;
            //return item;
            return null;
        }

        private CheckableListBoxItem GetSelectedItem()
        {
            //var item = ToolBarsPage.ToolBarListBox.SelectedItem as CheckableListBoxItem;
            //return item;
            return null;
        }

        private void OnItemChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            //var item = sender as CheckableListBoxItem;
            //var toolBar = item?.DataContext as ToolBar;
            //if (toolBar != null)
            //    ToolBarHostViewModel.ChangeToolBarVisibility(toolBar.IdentifierName, true);
        }

        private void OnItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var item = GetSelectedItem();
            //var toolBar = item?.DataContext as ToolBar;
            //if (toolBar == null)
            //    return;
            //var td = ToolBarHostViewModel.GetToolBarPosition(toolBar.IdentifierName);
            //switch (td)
            //{
            //    case Dock.Top:
            //        ToolBarsPage.RadioButtonTop.IsChecked = true;
            //        break;
            //    case Dock.Left:
            //        ToolBarsPage.RadioButtonLeft.IsChecked = true;
            //        break;
            //    case Dock.Right:
            //        ToolBarsPage.RadioButtonRight.IsChecked = true;
            //        break;
            //    default:
            //        ToolBarsPage.RadioButtonBottom.IsChecked = true;
            //        break;
            //}
        }

        private void OnItemUnchecked(object sender, RoutedEventArgs e)
        {
            //var item = sender as CheckableListBoxItem;
            //var toolBar = item?.DataContext as ToolBar;
            //if (toolBar != null)
            //    ToolBarHostViewModel.ChangeToolBarVisibility(toolBar.IdentifierName, false);
        }

        private void SetupRadioButtons()
        {
            //ToolBarsPage.RadioButtonTop.DataContext = Dock.Top;
            //ToolBarsPage.RadioButtonLeft.DataContext = Dock.Left;
            //ToolBarsPage.RadioButtonRight.DataContext = Dock.Right;
            //ToolBarsPage.RadioButtonBottom.DataContext = Dock.Bottom;

            //ToolBarsPage.RadioButtonTop.Command = RadioButtonCheckedCommand;
            //ToolBarsPage.RadioButtonTop.CommandParameter = ToolBarsPage.RadioButtonTop;

            //ToolBarsPage.RadioButtonRight.Command = RadioButtonCheckedCommand;
            //ToolBarsPage.RadioButtonRight.CommandParameter = ToolBarsPage.RadioButtonRight;

            //ToolBarsPage.RadioButtonLeft.Command = RadioButtonCheckedCommand;
            //ToolBarsPage.RadioButtonLeft.CommandParameter = ToolBarsPage.RadioButtonLeft;

            //ToolBarsPage.RadioButtonBottom.Command = RadioButtonCheckedCommand;
            //ToolBarsPage.RadioButtonBottom.CommandParameter = ToolBarsPage.RadioButtonBottom;
        }

        private void ToolBarsPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if ( ToolBarHostViewModel == null)
            //    return;
            //var toolBars = ToolBarHostViewModel.GetToolBars();
            //foreach (var toolBar in toolBars)
            //    ToolBarsPage.ToolBarListBox.Items.Add(CreateItem(toolBar));
            //ToolBarsPage.ToolBarListBox.Focus();
            //ToolBarsPage.Loaded -= ToolBarsPage_Loaded;
        }

        #region Commands

        public Command<RadioButton> RadioButtonCheckedCommand
            => new Command<RadioButton>(RadioButtonChecked, CanRadioButtonChecked);

        private void RadioButtonChecked(RadioButton button)
        {
            //if (button == null)
            //    return;
            //var item = GetSelectedItem();
            //var toolBar = item?.DataContext as ToolBar;
            //if (toolBar == null)
            //    return;

            //var dock = (Dock) button.DataContext;
            //ToolBarHostViewModel.ChangeToolBarPosition(toolBar.IdentifierName, dock);
        }

        private bool CanRadioButtonChecked(RadioButton button)
        {
            return true;
        }

        #endregion
    }
}