﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Focus
{
    public class CommandFocusManager
    {
        private static readonly List<Type> RegisteredCommandFocusElementTypes = new List<Type>();
        private static PresentationSource _currentMenuModeSource;
        private static RestoreFocusScope _restoreFocusScope;
        private static bool _isChecking;

        private static PresentationSource CurrentMenuModeSource
        {
            set
            {
                if (_currentMenuModeSource == value)
                    return;
                var currentMenuModeSource = _currentMenuModeSource;
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

        public static void CancelRestoreFocus()
        {
            _restoreFocusScope = null;
        }


        public static void Initialize()
        {
            CommandBarNavigationHelper.CommandFocusModePropertyChanged += OnCommandFocusModePropertyChanged;
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
            RegisterClassHandlers(typeof(Menu));
            RegisterClassHandlers(typeof(MenuItem));
            RegisterClassHandlers(typeof(ContextMenu));
            RegisterClassHandlers(typeof(ToolBar));
            EventManager.RegisterClassHandler(typeof(UIElement), System.Windows.Controls.ContextMenu.ClosedEvent,
                new RoutedEventHandler(OnContextMenuClosed));

            InputManager.Current.LeaveMenuMode += (param1, param2) => CorrectDetachedHwndFocus();
            Application.Current.MainWindow?.AddHandler(CommandManager.PreviewCanExecuteEvent, new CanExecuteRoutedEventHandler(CorrectFocusLookup));
        }

        private static void CorrectFocusLookup(object sender, CanExecuteRoutedEventArgs e)
        {
            if (InputManager.Current.IsInMenuMode)
            {
                if (e.Command is RoutedCommand routed)
                {
                    if (_isChecking)
                        return;
                    _isChecking = true;


                    if (MenuModeHelper.CommandModeKind == CommandBarModeKind.SearchControl)
                    {
                        e.CanExecute = routed.CanExecute(e.Parameter, null);
                    }
                    else
                    {
                        e.CanExecute = routed.CanExecute(e.Parameter, _restoreFocusScope?.RestoreFocus);
                    }
                    _isChecking = false;

                    // I'm not sure if this can be ommited but apparently it does not break anything.
                    // Omitting fixes menus not beeing updated when entering menu mode in searchcontrols
                    // EDIT: Omitting causes Search controls sometime not beeing able to use backspace/space key
                    if (e.CanExecute)
                        e.Handled = true;
                }
            }
        }

        internal static bool IsAttachedCommandFocusElement(DependencyObject element)
        {
            return CommandBarNavigationHelper.GetCommandFocusMode(element) ==
                   CommandBarNavigationHelper.CommandFocusMode.Attached;
        }

        internal static bool IsInsideAttachedCommandFocusElement(IInputElement inputElement)
        {
            if (!(inputElement is DependencyObject dependencyObject1))
                return false;
            var dependencyObject2 = dependencyObject1;
            Func<DependencyObject, DependencyObject> parentEvaluator = Utilities.ExtensionMethods.GetVisualOrLogicalParent;
            Func<DependencyObject, bool> func = IsAttachedCommandFocusElement;
            return dependencyObject2.FindAncestorOrSelf(parentEvaluator, func) != null;
        }

        internal static bool IsInsideCommandContainer(IInputElement inputElement)
        {
            if (!(inputElement is DependencyObject dependencyObject1))
                return false;
            var dependencyObject2 = dependencyObject1;
            Func<DependencyObject, DependencyObject> parentEvaluator = Utilities.ExtensionMethods.GetVisualOrLogicalParent;
            Func<DependencyObject, bool> func = IsRegisteredCommandFocusElement;
            return dependencyObject2.FindAncestorOrSelf(parentEvaluator, func) != null;
        }

        internal static bool IsRegisteredCommandFocusElement(DependencyObject element)
        {
            if (!(element is Menu) && !(element is ContextMenu) && !(element is ToolBar))
                return (uint)CommandBarNavigationHelper.GetCommandFocusMode(element) > 0U;
            return true;
        }

        private static void CorrectDetachedHwndFocus()
        {
            if (!(Keyboard.FocusedElement is DependencyObject focusedElement))
                return;
            focusedElement.AcquireWin32Focus(out _);
        }

        private static bool IsCommandContainerGainingFocus(IInputElement oldFocus, IInputElement newFocus)
        {
            if (!IsInsideCommandContainer(newFocus))
                return false;
            if (oldFocus != null)
                return !IsInsideCommandContainer(oldFocus);
            return true;
        }

        private static bool IsCommandContainerLosingFocus(IInputElement oldFocus, IInputElement newFocus)
        {
            return IsCommandContainerGainingFocus(newFocus, oldFocus);
        }

        private static bool IsCurrentThreadMainUiThread()
        {
            return Application.Current.Dispatcher.CheckAccess();
        }

        private static void OnCommandFocusModePropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var newValue = (CommandBarNavigationHelper.CommandFocusMode)e.NewValue;
            var type = obj.GetType();
            if (newValue == CommandBarNavigationHelper.CommandFocusMode.None ||
                RegisteredCommandFocusElementTypes.Contains(type) || type.IsEquivalentTo(typeof(Menu)) ||
                type.IsSubclassOf(typeof(Menu)) || type.IsEquivalentTo(typeof(ContextMenu)) ||
                type.IsSubclassOf(typeof(ContextMenu)) || type.IsEquivalentTo(typeof(ToolBar)) ||
                type.IsSubclassOf(typeof(ToolBar)))
                return;
            foreach (var focusElementType in RegisteredCommandFocusElementTypes)
                if (focusElementType.IsSubclassOf(type) || type.IsSubclassOf(focusElementType))
                    return;
            RegisteredCommandFocusElementTypes.Add(type);
            RegisterClassHandlers(type);
        }

        private static void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            if (!IsCurrentThreadMainUiThread() || _restoreFocusScope == null ||
                !IsCommandContainerLosingFocus(e.Source as IInputElement, Keyboard.FocusedElement))
                return;
            CurrentMenuModeSource = null;
            var restoreFocusScope = _restoreFocusScope;
            _restoreFocusScope = null;
            restoreFocusScope.PerformRestoration();
        }

        private static void OnKeyboardInputProviderAcquireFocus(object sender,
            KeyboardInputProviderAcquireFocusEventArgs e)
        {
            if (!IsCurrentThreadMainUiThread())
                return;
            if (!(sender is DependencyObject dependencyObject) || !IsRegisteredCommandFocusElement(dependencyObject))
                return;
            if (dependencyObject is IInputElement inputElement && !inputElement.IsKeyboardFocusWithin)
            {
                if (e.RoutedEvent == Keyboard.PreviewKeyboardInputProviderAcquireFocusEvent)
                {
                    if (!IsAttachedCommandFocusElement(dependencyObject))
                        CurrentMenuModeSource = PresentationSource.FromDependencyObject(dependencyObject);
                }
                else if (!e.FocusAcquired)
                    CurrentMenuModeSource = null;
            }
            if (PresentationSource.FromDependencyObject(dependencyObject) != null)
                return;
            e.Handled = true;
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

        private static void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            if (!IsCurrentThreadMainUiThread())
                return;

            if (!(sender is DependencyObject dependencyObject) || !IsRegisteredCommandFocusElement(dependencyObject))
                return;

            if (_restoreFocusScope == null && IsCommandContainerGainingFocus(args.OldFocus, args.NewFocus))
            {
                if (!IsAttachedCommandFocusElement(dependencyObject))
                    CurrentMenuModeSource = PresentationSource.FromDependencyObject(dependencyObject);
                _restoreFocusScope = new CommandRestoreFocusScope(args.OldFocus);
            }
            if (PresentationSource.FromDependencyObject(dependencyObject) != null)
                return;
            args.Handled = true;
        }

        private static void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            if (!IsCurrentThreadMainUiThread())
                return;
            if (!(sender is DependencyObject element) || !IsRegisteredCommandFocusElement(element) ||
                _restoreFocusScope == null || !IsCommandContainerLosingFocus(args.OldFocus, args.NewFocus) ||
                args.NewFocus == null)
                return;
            var restoreFocusScope = _restoreFocusScope;
            _restoreFocusScope = null;
            if (IsAttachedCommandFocusElement(element))
                return;

            if (((FrameworkElement)sender).FindAncestor<AnchorableToolBarTray>() == null)
                restoreFocusScope.PerformRestoration();
            args.Handled = true;
        }

        private static void RegisterClassHandlers(Type type)
        {
            EventManager.RegisterClassHandler(type, Keyboard.PreviewKeyboardInputProviderAcquireFocusEvent,
                new KeyboardInputProviderAcquireFocusEventHandler(OnKeyboardInputProviderAcquireFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.KeyboardInputProviderAcquireFocusEvent,
                new KeyboardInputProviderAcquireFocusEventHandler(OnKeyboardInputProviderAcquireFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.PreviewGotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(OnPreviewGotKeyboardFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.PreviewLostKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(OnPreviewLostKeyboardFocus), true);
            EventManager.RegisterClassHandler(type, Keyboard.LostKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(OnLostKeyboardFocus), true);
        }
    }
}