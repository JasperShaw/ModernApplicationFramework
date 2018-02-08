using System;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.WindowModels
{
    /// <inheritdoc cref="IMainWindowViewModel" />
    /// <summary>
    /// View model for a <see cref="MainWindow"/> that is an instance of <see cref="Conductor{T}.Collection.OneActive"/> where T is <see langword="object"/>
    /// </summary>
    /// <seealso cref="Conductor{T}.Collection.OneActive" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IMainWindowViewModel" />
    public class MainWindowViewModelConductorOneActive : WindowViewModelConductorOneActive, IMainWindowViewModel
    {
        private IMenuHostViewModel _menuHostViewModel;
        private IToolBarHostViewModel _toolBarHostViewModel;
        private bool _useTitleBar;
        private bool _useMenu;
        private IInfoBarHost _infoBarHost;

        private bool MenuHostViewModelSetted => MenuHostViewModel != null;

        private bool ToolbarHostViewModelSetted => ToolBarHostViewModel != null;

        public bool InfobarHostViewSetted => InfoBarHost != null;


        /// <inheritdoc />
        /// <summary>
        ///     Contains the ViewModel of the MainWindows MenuHostControl
        ///     This can not be changed once it was setted with a value.
        /// </summary>
        public IMenuHostViewModel MenuHostViewModel
        {
            get => _menuHostViewModel;
            set
            {
                if (MenuHostViewModelSetted)
                    throw new InvalidOperationException("You can not change the MenuHostViewModel once it was setted up");
                _menuHostViewModel = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Contains the ViewModel of the MainWindows ToolbarHostControl
        ///     This can not be changed once it was setted with a value
        /// </summary>
        public IToolBarHostViewModel ToolBarHostViewModel
        {
            get => _toolBarHostViewModel;
            set
            {
                if (ToolbarHostViewModelSetted)
                    throw new InvalidOperationException(
                        "You can not change the ToolBarHostViewModel once it was setted up");
                _toolBarHostViewModel = value;
            }
        }

        public IInfoBarHost InfoBarHost
        {
            get => _infoBarHost;
            set
            {
                if (InfobarHostViewSetted)
                    throw new InvalidOperationException(
                        "You can not change the InfoBarHost once it was setted up");
                _infoBarHost = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Contains information whether a TitleBar is displayed or not
        /// </summary>
        public bool UseTitleBar
        {
            get => _useTitleBar;
            set
            {
                if (Equals(value, _useTitleBar))
                    return;
                _useTitleBar = value;
                NotifyOfPropertyChange();
                //NotifyOfPropertyChange(() => UseTitleBar);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Option to use a menu bar or not
        /// </summary>
        public bool UseMenu
        {
            get => _useMenu;
            set
            {
                if (Equals(value, _useMenu))
                    return;
                _useMenu = value;
                NotifyOfPropertyChange();
            }
        }
    }
}