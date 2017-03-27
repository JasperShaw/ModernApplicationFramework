using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Interfaces.Views;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [Export(typeof(NewToolBarDialogViewModel))]
    internal sealed class NewToolBarDialogViewModel : Screen
    {
        private string _toolbarName;
        private INewToolBarView _toolbarView;

        public NewToolBarDialogViewModel()
        {
            DisplayName = "New Toolbar";
            ToolbarName = IoC.Get<IToolBarHostViewModel>().GetUniqueToolBarName();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is INewToolBarView correctView)
                _toolbarView = correctView;
        }

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

        public Command OkClickCommand => new Command(ExecuteOkClick);

        private void ExecuteOkClick()
        {
            string errorMessage = null;
            var name = ToolbarName?.Trim();
            if (string.IsNullOrEmpty(name))
                errorMessage = "The toolbar name cannot be blank. Type a name.";
            else
            {
                foreach (var definition in IoC.Get<IToolBarHostViewModel>().ToolbarDefinitions)
                {
                    var defName = definition.Text;
                    if (!name.Equals(defName, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    errorMessage = $"A toolbar named '{defName}' already exists. Type another name.";
                    break;
                }
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
