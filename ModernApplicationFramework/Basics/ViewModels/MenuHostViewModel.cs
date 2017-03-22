using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.ViewModels
{
    [Export(typeof(IMenuHostViewModel))]
    public class MenuHostViewModel : ViewModelBase, IMenuHostViewModel
    {
        private IMainWindowViewModel _mainWindowViewModel;
        private MenuHostControl _menuHostControl;
        private readonly IToolBarHostViewModel _toolBarHost;

        public MenuHostViewModel()
        {
            Items = new BindableCollection<MenuItem>();
            _toolBarHost = IoC.Get<IToolBarHostViewModel>();
        }

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        ///     Default is true
        /// </summary>
        public bool AllowOpenToolBarContextMenu { get; set; } = true;

        internal MenuHostControl MenuHostControl
        {
            get => _menuHostControl;
            set
            {
                if (_menuHostControl != null)
                    _menuHostControl.MouseRightButtonDown -= _control_MouseRightButtonDown;
                _menuHostControl = value;
                _menuHostControl.MouseRightButtonDown += _control_MouseRightButtonDown;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Contains the Items of the MenuHostControl
        /// </summary>
        public ObservableCollection<MenuItem> Items { get; set; }

        /// <summary>
        ///     Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        public IMainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set
            {
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = value;
            }
        }

        public void CreateMenu(IMenuCreator creator)
        {
            creator.CreateMenu(this);
        }

        public Command RightClickCommand => new Command(ExecuteRightClick);

        private async void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await RightClickCommand.Execute();
        }

        protected virtual async void ExecuteRightClick()
        {
            if (_toolBarHost == null)
                return;

            if (AllowOpenToolBarContextMenu && _toolBarHost.ToolbarDefinitions.Any())
                await _toolBarHost.OpenContextMenuCommand.Execute();
        }
    }
}