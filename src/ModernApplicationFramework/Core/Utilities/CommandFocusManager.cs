using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;
using ContextMenu = ModernApplicationFramework.Controls.Menu.ContextMenu;
using Menu = ModernApplicationFramework.Controls.Menu.Menu;
using MenuItem = ModernApplicationFramework.Controls.Menu.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.Core.Utilities
{
    public class CommandFocusManager
    {
        private static readonly List<Type> RegisteredCommandFocusElementTypes = new List<Type>();
        private static RestoreFocusScope _restoreFocusScope;
        private static PresentationSource _currentMenuModeSource;

        private static PresentationSource CurrentMenuModeSource
        {
            set
            {
                if (_currentMenuModeSource == value)
                    return;
                PresentationSource currentMenuModeSource = _currentMenuModeSource;
                _currentMenuModeSource = value;
                try
                {
                    if (_currentMenuModeSource == null)
                        return;
                    InputManager.Current.PushMenuMode(_currentMenuModeSource);
                }
                finally
                {
                    if (currentMenuModeSource != null)
                        InputManager.Current.PopMenuMode(currentMenuModeSource);
                }
            }
        }


        public static void Initialize()
        {
            CommandNavigationHelper.CommandFocusModePropertyChanged += OnCommandFocusModePropertyChanged;
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
            RegisterClassHandlers(typeof(Menu));
            RegisterClassHandlers(typeof(ContextMenu));
            RegisterClassHandlers(typeof(ToolBar));
            EventManager.RegisterClassHandler(typeof(UIElement), System.Windows.Controls.ContextMenu.ClosedEvent, new RoutedEventHandler(OnContextMenuClosed));
            InputManager.Current.LeaveMenuMode += (param1, param2) => CorrectDetachedHwndFocus();
        }

        private static void OnCommandFocusModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CommandNavigationHelper.CommandFocusMode newValue = (CommandNavigationHelper.CommandFocusMode)e.NewValue;
            Type type = obj.GetType();
            if (newValue == CommandNavigationHelper.CommandFocusMode.None || RegisteredCommandFocusElementTypes.Contains(type) || (type.IsEquivalentTo(typeof(Menu)) || type.IsSubclassOf(typeof(Menu))) || type.IsEquivalentTo(typeof(ContextMenu)) || type.IsSubclassOf(typeof(ContextMenu)) || type.IsEquivalentTo(typeof(ToolBar)) || type.IsSubclassOf(typeof(ToolBar)))
                return;
            foreach (Type focusElementType in RegisteredCommandFocusElementTypes)
            {
                if (focusElementType.IsSubclassOf(type) || type.IsSubclassOf(focusElementType))
                    return;
            }
            RegisteredCommandFocusElementTypes.Add(type);
            RegisterClassHandlers(type);
        }


        private static void RegisterClassHandlers(Type type)
        {
            EventManager.RegisterClassHandler(type, Keyboard.PreviewKeyboardInputProviderAcquireFocusEvent, new KeyboardInputProviderAcquireFocusEventHandler(OnKeyboardInputProviderAcquireFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.KeyboardInputProviderAcquireFocusEvent, new KeyboardInputProviderAcquireFocusEventHandler(OnKeyboardInputProviderAcquireFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.PreviewGotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnPreviewGotKeyboardFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.PreviewLostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnPreviewLostKeyboardFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnLostKeyboardFocus), true);
        }

        private static void OnKeyboardInputProviderAcquireFocus(object sender, KeyboardInputProviderAcquireFocusEventArgs e)
        {
            if (!IsCurrentThreadMainUiThread())
                return;
            if (!(sender is DependencyObject dependencyObject) || !IsRegisteredCommandFocusElement(dependencyObject))
                return;
            if (!(dependencyObject is IInputElement inputElement) || inputElement.IsKeyboardFocusWithin)
                return;
            if (e.RoutedEvent == Keyboard.PreviewKeyboardInputProviderAcquireFocusEvent)
            {
                if (IsAttachedCommandFocusElement(dependencyObject))
                    return;
                CurrentMenuModeSource = PresentationSource.FromDependencyObject(dependencyObject);
            }
            else
            {
                if (e.FocusAcquired)
                    return;
                CurrentMenuModeSource = null;
            }
        }

        internal static bool IsAttachedCommandFocusElement(DependencyObject element)
        {
            return CommandNavigationHelper.GetCommandFocusMode(element) == CommandNavigationHelper.CommandFocusMode.Attached;
        }

        private static void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            if (!IsCurrentThreadMainUiThread())
                return;
            if (!(sender is DependencyObject element) || !IsRegisteredCommandFocusElement(element) || (_restoreFocusScope == null || !IsCommandContainerLosingFocus(args.OldFocus, args.NewFocus)) || args.NewFocus == null)
                return;
            RestoreFocusScope restoreFocusScope = _restoreFocusScope;
            _restoreFocusScope = null;
            if (IsAttachedCommandFocusElement(element))
                return;
            restoreFocusScope.PerformRestoration();
            args.Handled = true;
        }

        private static void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            if (!IsCurrentThreadMainUiThread())
                return;
            if (!(sender is DependencyObject element) || !IsRegisteredCommandFocusElement(element))
                return;
            if (IsCommandContainerLosingFocus(args.OldFocus, args.NewFocus))
            {
                CurrentMenuModeSource = null;
                if (args.NewFocus == null)
                    return;
                CancelRestoreFocus();
            }
            else
            {
                if (!IsInsideAttachedCommandFocusElement(args.NewFocus))
                    return;
                CurrentMenuModeSource = null;
            }
        }

        internal static bool IsInsideAttachedCommandFocusElement(IInputElement inputElement)
        {
            if (!(inputElement is DependencyObject dependencyObject1))
                return false;
            DependencyObject dependencyObject2 = dependencyObject1;
            Func<DependencyObject, DependencyObject> parentEvaluator = ModernApplicationFramework.Utilities.ExtensionMethods.GetVisualOrLogicalParent;
            Func<DependencyObject, bool> func = IsAttachedCommandFocusElement;
            return dependencyObject2.FindAncestorOrSelf(parentEvaluator, func) != null;
        }

        public static void CancelRestoreFocus()
        {
            _restoreFocusScope = null;
        }

        private static void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            if (!IsCurrentThreadMainUiThread())
                return;
            if (!(sender is DependencyObject dependencyObject) || !IsRegisteredCommandFocusElement(dependencyObject) || _restoreFocusScope != null || !IsCommandContainerGainingFocus(args.OldFocus, args.NewFocus))
                return;
            if (!IsAttachedCommandFocusElement(dependencyObject))
                CurrentMenuModeSource = PresentationSource.FromDependencyObject(dependencyObject);
            IntPtr restoreFocusWindow = args.OldFocus != null ? IntPtr.Zero : User32.GetFocus();
            IntPtr expectedDefocusWindow = ComputeExpectedDefocusWindow(args.OldFocus as DependencyObject);
            _restoreFocusScope = new CommandRestoreFocusScope(args.OldFocus, restoreFocusWindow, expectedDefocusWindow);
            PreventFocusScopeCommandRedirection(args.NewFocus as DependencyObject);
        }

        private static void PreventFocusScopeCommandRedirection(DependencyObject newFocus)
        {
            if (newFocus == null)
                return;
            DependencyObject parentFocusScope = GetParentFocusScope(FocusManager.GetFocusScope(newFocus));
            if (parentFocusScope == null)
                return;
            FocusManager.SetFocusedElement(parentFocusScope, null);
        }

        private static DependencyObject GetParentFocusScope(DependencyObject focusScope)
        {
            DependencyObject visualOrLogicalParent = focusScope?.GetVisualOrLogicalParent();
            if (visualOrLogicalParent != null)
                return FocusManager.GetFocusScope(visualOrLogicalParent);
            return null;
        }

        private static IntPtr ComputeExpectedDefocusWindow(DependencyObject oldFocus)
        {
            if (oldFocus != null)
            {
                if (!(PresentationSource.FromDependencyObject(oldFocus) is HwndSource hwndSource))
                    return IntPtr.Zero;
                return hwndSource.Handle;
            }
            IntPtr hWnd = User32.GetFocus();
            if (hWnd == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr desktopWindow = User32.GetDesktopWindow();
            while (true)
            {
                IntPtr ancestor = User32.GetAncestor(hWnd, 1);
                if (!(ancestor == IntPtr.Zero) && !(ancestor == desktopWindow))
                    hWnd = ancestor;
                else
                    break;
            }
            return hWnd;
        }

        private static void CorrectDetachedHwndFocus()
        {
            if (!(Keyboard.FocusedElement is DependencyObject focusedElement))
                return;
            focusedElement.AcquireWin32Focus(out var _);
        }

        private static void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            if (!IsCurrentThreadMainUiThread() || _restoreFocusScope == null || !IsCommandContainerLosingFocus(e.Source as IInputElement, Keyboard.FocusedElement))
                return;
            CurrentMenuModeSource = null;
            RestoreFocusScope restoreFocusScope = _restoreFocusScope;
            _restoreFocusScope = null;
            restoreFocusScope.PerformRestoration();
        }

        private static bool IsCurrentThreadMainUiThread()
        {
            return Application.Current.Dispatcher.CheckAccess();
        }

        private static bool IsCommandContainerLosingFocus(IInputElement oldFocus, IInputElement newFocus)
        {
            return IsCommandContainerGainingFocus(newFocus, oldFocus);
        }

        private static bool IsCommandContainerGainingFocus(IInputElement oldFocus, IInputElement newFocus)
        {
            if (!IsInsideCommandContainer(newFocus))
                return false;
            if (oldFocus != null)
                return !IsInsideCommandContainer(oldFocus);
            return true;
        }

        internal static bool IsInsideCommandContainer(IInputElement inputElement)
        {
            if (!(inputElement is DependencyObject dependencyObject1))
                return false;
            DependencyObject dependencyObject2 = dependencyObject1;
            Func<DependencyObject, DependencyObject> parentEvaluator = ModernApplicationFramework.Utilities.ExtensionMethods.GetVisualOrLogicalParent;
            Func<DependencyObject, bool> func = IsRegisteredCommandFocusElement;
            return dependencyObject2.FindAncestorOrSelf(parentEvaluator, func) != null;
        }

        internal static bool IsRegisteredCommandFocusElement(DependencyObject element)
        {
            if (!(element is Menu) && !(element is ContextMenu) && !(element is ToolBar))
                return (uint)CommandNavigationHelper.GetCommandFocusMode(element) > 0U;
            return true;
        }



    }


    internal class CommandRestoreFocusScope : RestoreFocusScope
    {
        private readonly IntPtr _expectedFocusWindowBeforeRestore;

        public CommandRestoreFocusScope(IInputElement restoreFocus, IntPtr restoreFocusWindow, IntPtr expectedFocusWindowBeforeRestore)
            : base(restoreFocus, restoreFocusWindow)
        {
            _expectedFocusWindowBeforeRestore = expectedFocusWindowBeforeRestore;
        }

        public override void PerformRestoration()
        {
            if (!ShouldPerformRestoration)
                return;
            base.PerformRestoration();
            if (RestoreFocus != null)
                return;
            Keyboard.ClearFocus();
        }

        protected override bool IsRestorationTargetValid()
        {
            return true;
        }

        private bool ShouldPerformRestoration => RestoreFocusWindow == IntPtr.Zero || _expectedFocusWindowBeforeRestore == User32.GetFocus() || _expectedFocusWindowBeforeRestore == IntPtr.Zero;
    }





    internal abstract class RestoreFocusScope
    {
        private List<DependencyObject> _focusAncestors;

        protected RestoreFocusScope(IInputElement restoreFocus, IntPtr restoreFocusWindow)
        {
            RestoreFocusWindow = restoreFocusWindow;
            RestoreFocus = restoreFocus;
        }

        protected abstract bool IsRestorationTargetValid();

        public virtual void PerformRestoration()
        {
            if (!IsRestorationTargetValid())
                return;
            if (RestoreFocusWindow != IntPtr.Zero)
            {
                User32.SetFocus(RestoreFocusWindow);
            }
            else
            {
                if (RestoreFocus == null)
                    return;
                Keyboard.Focus(RestoreFocus);
            }
        }

        protected IInputElement RestoreFocus
        {
            get
            {
                IInputElement inputElement = null;
                if (_focusAncestors != null)
                    inputElement = _focusAncestors.Find(e => e.IsConnectedToPresentationSource()) as IInputElement;
                return inputElement;
            }
            private set
            {
                _focusAncestors = new List<DependencyObject>();
                for (DependencyObject sourceElement = value as DependencyObject; sourceElement != null; sourceElement = sourceElement.GetVisualOrLogicalParent())
                {
                    IInputElement inputElement;
                    if ((inputElement = sourceElement as IInputElement) != null && inputElement.Focusable)
                        _focusAncestors.Add(sourceElement);
                }
            }
        }

        protected IntPtr RestoreFocusWindow { get; }
    }




    public static class CommandModeHelper
    {
        private static readonly WeakCollection<Menu> MainMenus = new WeakCollection<Menu>();
        private static IEnumerable<FrameworkElement> trackingCommandModeFocusableElements;
       
        private static CommandModeState commandState = new CommandModeState(TryEnterMenuMode, TryEnterToolBarMode);
        private static Key _lastKeyDown;

        private static bool IsModal { get; set; }

        public static void RegisterMenu(Menu menu)
        {
            MainMenus.Add(menu);
        }
        private static bool TryEnterMenuMode()
        {
            if (IsModal)
                return TryEnterToolBarMode();
            return MainMenus.Any(TryEnterItemsControl<MenuItem>);
        }

        public static bool TranslateAccelerator(MSG msg)
        {
            return TranslateAccelerator(msg, new ToolBarTray[0], false);
        }



        public static bool TranslateAccelerator(Key key, bool up)
        {
            ModifierKeys modifierKeys = NativeMethods.ModifierKeys;

            bool isRepeat = key == _lastKeyDown;
            _lastKeyDown = key;

            if (!up)
                return commandState.FilterKeyDownMessage(key, modifierKeys, isRepeat);
            return commandState.FilterKeyUpMessage(key);
        }




        public static bool TranslateAccelerator(MSG msg,
            IEnumerable<FrameworkElement> activeCommandModeFocusableElements, bool isModal)
        {

            trackingCommandModeFocusableElements = activeCommandModeFocusableElements;
            IsModal = isModal;
            bool flag = false;
            if (IsMouseClick(msg.message))
                flag = commandState.FilterMouseMessage();

            else if (IsKeyDownMessage(msg.message))
            {
                ModifierKeys modifierKeys = NativeMethods.ModifierKeys;


                var realKey = Key.RightAlt;

                bool isRepeat = realKey == _lastKeyDown;
                _lastKeyDown = realKey;

                flag = commandState.FilterKeyDownMessage(realKey, modifierKeys, isRepeat);

            }
            else if (IsKeyUpMessage(msg.message))
            {
                Key realKey = KeyInterop.KeyFromVirtualKey(msg.wParam.ToInt32());
                flag = commandState.FilterKeyUpMessage(realKey);
            }
            trackingCommandModeFocusableElements = null;
            return flag;
        }

        private static bool IsKeyUpMessage(int message)
        {
            if (message != 257)
                return message == 261;
            return true;
        }

        private static bool IsKeyDownMessage(int message)
        {
            if (message != 256)
                return message == 260;
            return true;
        }

        private static bool IsMouseClick(int message)
        {
            if (message >= 512 && message <= 525 && message != 512)
                return message != 522;
            return false;
        }

        internal static CommandModeKind CommandModeKind
        {
            get
            {
                if (Keyboard.FocusedElement is DependencyObject focusedElement)
                {
                    DependencyObject dependencyObject1 = focusedElement;
                    DependencyObject Func1(DependencyObject e) => e.GetVisualOrLogicalParent();
                    if (dependencyObject1.FindAncestorOrSelf<System.Windows.Controls.Menu, DependencyObject>(Func1) != null)
                        return CommandModeKind.Menu;
                    DependencyObject dependencyObject2 = focusedElement;
                    DependencyObject Func2(DependencyObject e) => e.GetVisualOrLogicalParent();
                    if (dependencyObject2.FindAncestorOrSelf<System.Windows.Controls.ToolBar, DependencyObject>(Func2) != null)
                        return CommandModeKind.Toolbar;
                }
                return CommandModeKind.None;
            }
        }

        private static bool TryEnterToolBarMode()
        {
            foreach (FrameworkElement focusableElement in trackingCommandModeFocusableElements)
            {
                if (focusableElement is ToolBarTray tray)
                {
                    foreach (System.Windows.Controls.ToolBar sortedToolBar in GetSortedToolBars(tray))
                    {
                        if (TryEnterItemsControl<Control>(sortedToolBar))
                            return true;
                    }
                }
                else if (TryFocusElementOrDescendant(focusableElement))
                    return true;
            }
            return false;
        }

        private static IEnumerable<System.Windows.Controls.ToolBar> GetSortedToolBars(ToolBarTray tray)
        {
            List<System.Windows.Controls.ToolBar> toolBarList = new List<System.Windows.Controls.ToolBar>(tray.ToolBars);
            toolBarList.Sort((tb1, tb2) =>
            {
                if (tb1.Band == tb2.Band)
                    return tb1.BandIndex.CompareTo(tb2.BandIndex);
                return tb1.Band.CompareTo(tb2.Band);
            });
            return toolBarList;
        }

        private static bool TryFocusElementOrDescendant(UIElement element)
        {
            element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            return element.IsKeyboardFocusWithin;
        }

        private static bool TryEnterItemsControl<TItem>(ItemsControl itemsControl) where TItem : UIElement
        {
            for (int i = 0; i < itemsControl.Items.Count; ++i)
            {
                if (TryFocusItem<TItem>(itemsControl, i))
                    return true;
            }
            return false;
        }

        private static bool TryFocusItem<TItem>(ItemsControl itemsControl, int i) where TItem : UIElement
        {
            if (itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is TItem obj && !(itemsControl.Items[i] is Separator))
            {
                obj.Focus();
                if (obj.IsKeyboardFocusWithin)
                    return true;
            }
            return false;
        }

        public static bool ProcessAccessKey(string key, List<object> scopes)
        {
            if (scopes == null)
                throw new ArgumentNullException(nameof(scopes));
            if (key.Length != 1)
                return false;
            foreach (object scope in scopes)
            {
                if (AccessKeyManager.IsKeyRegistered(scope, key))
                {
                    AccessKeyManager.ProcessKey(scope, key, false);
                    return true;
                }
            }
            return false;
        }

        public static void ResetState()
        {
            commandState.ResetState();
        }
    }

    internal class CommandModeState
    {
        public Key LastKeyPressed;
        private ModifierKeys _lastModifierKeys;
        private bool _win32MenuModeWorkAround;
        private bool _shouldEnterToolBarMode;

        private Func<bool> TryEnterToolBarMode { get; }

        private Func<bool> TryEnterMenuMode { get; }


        public CommandModeState(Func<bool> enterMenuMode, Func<bool> enterToolBarMode)
        {
            TryEnterMenuMode = enterMenuMode;
            TryEnterToolBarMode = enterToolBarMode;
        }

        private void ClearLastKey()
        {
            LastKeyPressed = Key.None;
            _shouldEnterToolBarMode = false;
        }

        private static bool IsMenuKey(Key key)
        {
            if (key == Key.System)
                return true;
            if (key != Key.LeftAlt && key != Key.RightAlt)
                return key == Key.F10;
            return true;
        }

        public bool FilterMouseMessage()
        {
            ClearLastKey();
            _win32MenuModeWorkAround = false;
            return false;
        }

        private bool ShouldReenterMenuCommandMode(Key realKey, ModifierKeys modifierKeys)
        {
            return (realKey == Key.LeftAlt || realKey == Key.RightAlt) && modifierKeys == ModifierKeys.Alt || realKey == Key.F10 && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None;
        }

        public bool FilterKeyDownMessage(Key realKey, ModifierKeys modifierKeys, bool isRepeat)
        {
            CommandModeKind commandModeKind = CommandModeHelper.CommandModeKind;
            bool flag = commandModeKind != CommandModeKind.None && commandModeKind != CommandModeKind.Menu;
            if (commandModeKind == CommandModeKind.Menu || flag && !ShouldReenterMenuCommandMode(realKey, modifierKeys))
                ClearLastKey();
            else if (!isRepeat)
            {
                if (LastKeyPressed == Key.None)
                {
                    if (IsMenuKey(realKey) && !NativeMethods.IsLeftButtonPressed() && !NativeMethods.IsRightButtonPressed())
                    {
                        if ((modifierKeys & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows)) == ModifierKeys.Shift)
                        {
                            if (realKey != Key.F10)
                            {
                                LastKeyPressed = realKey;
                                _lastModifierKeys = modifierKeys;
                                _shouldEnterToolBarMode = true;
                            }
                        }
                        else if ((modifierKeys & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows)) == ModifierKeys.None)
                        {
                            LastKeyPressed = realKey;
                            _lastModifierKeys = modifierKeys;
                        }
                    }
                }
                else if (LastKeyPressed != realKey || _lastModifierKeys != modifierKeys)
                    ClearLastKey();
                _win32MenuModeWorkAround = false;
            }
            return false;
        }

        public bool FilterKeyUpMessage(Key realKey)
        {
            bool flag = false;
            if (realKey == LastKeyPressed && IsMenuKey(realKey))
            {
                if (_shouldEnterToolBarMode)
                    flag = TryEnterToolBarMode();
                if (!flag)
                    flag = TryEnterMenuMode();
            }
            if (_win32MenuModeWorkAround)
            {
                if (IsMenuKey(realKey))
                {
                    _win32MenuModeWorkAround = false;
                    flag = true;
                }
            }
            else if (flag)
                _win32MenuModeWorkAround = true;
            ClearLastKey();
            return flag;
        }

        public void ResetState()
        {
            ClearLastKey();
            _win32MenuModeWorkAround = false;
        }
    }

    internal enum CommandModeKind
    {
        None,
        Menu,
        Toolbar,
        //NavigableControl,
        //InfoBar,
        //SearchControl,
    }

}
