using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Interfaces.Views;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Data view model of the new tool bar dialog
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.Screen" />
    [Export(typeof(NewToolBarDialogViewModel))]
    internal sealed class NewToolBarDialogViewModel : Screen
    {
        private string _toolbarName;
        private INewToolBarView _toolbarView;

        /// <summary>
        /// The Command to perform the OK click
        /// </summary>
        public Command OkClickCommand => new Command(ExecuteOkClick);

        /// <summary>
        /// The name of the tool bar.
        /// </summary>
        public string ToolbarName
        {
            get => _toolbarName;
            set
            {
                if (value == _toolbarName)
                    return;
                _toolbarName = value;
                NotifyOfPropertyChange();
            }
        }

        public NewToolBarDialogViewModel()
        {
            DisplayName = Customize_Resources.NewToolbarPrompt_Title;
            ToolbarName = IoC.Get<IToolBarHostViewModel>().GetUniqueToolBarName();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is INewToolBarView correctView)
                _toolbarView = correctView;
        }

        private void ExecuteOkClick()
        {
            string errorMessage = null;
            var name = ToolbarName?.Trim();
            if (string.IsNullOrEmpty(name))
                errorMessage = Customize_Resources.Error_AddOrRenameToolbarEmptyName;
            else
                foreach (var definition in IoC.Get<IToolBarHostViewModel>().TopLevelDefinitions)
                {
                    var defName = definition.Text;
                    if (!name.Equals(defName, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    errorMessage = string.Format(CultureInfo.CurrentUICulture, Customize_Resources.Error_AddOrRenameToolbarExisting, defName);
                    break;
                }
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, Application.Current.MainWindow.Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                _toolbarView.SelectTextBox();
            }
            else
            {
                ToolbarName = name;
                TryClose(true);
            }
        }
    }
}