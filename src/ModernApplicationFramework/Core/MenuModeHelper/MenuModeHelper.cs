using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.MenuModeHelper
{
    internal static class MenuModeHelper
    {
        private static readonly WeakCollection<Menu> MainMenus = new WeakCollection<Menu>();
        private static readonly WeakCollection<ToolBarTray> MainToolBarTrays = new WeakCollection<ToolBarTray>();

        private static readonly CommandBarModeState CommandState =
            new CommandBarModeState(TryEnterMenuMode, TryEnterToolBarMode);

        private static readonly CommandBarNavigationState CommandBarNavigationState =
            new CommandBarNavigationState(NavigateCommandFocus);


        private static Key _lastKeyDown;

        internal static CommandBarModeKind CommandModeKind
        {
            get
            {
                if (Keyboard.FocusedElement is DependencyObject focusedElement)
                {
                    DependencyObject dependencyObject1 = focusedElement;
                    DependencyObject Func1(DependencyObject e) => e.GetVisualOrLogicalParent();
                    if (dependencyObject1.FindAncestorOrSelf<Menu, DependencyObject>(Func1) != null)
                        return CommandBarModeKind.Menu;
                    DependencyObject dependencyObject2 = focusedElement;
                    DependencyObject Func2(DependencyObject e) => e.GetVisualOrLogicalParent();
                    if (dependencyObject2.FindAncestorOrSelf<ToolBar, DependencyObject>(Func2) != null)
                        return CommandBarModeKind.Toolbar;
                    DependencyObject dependencyObject3 = focusedElement;
                    DependencyObject Func3(DependencyObject e) => e.GetVisualOrLogicalParent();
                    bool Func4(DependencyObject e) => e is IInfoBarHost;
                    if (dependencyObject3.FindAncestorOrSelf(Func3, Func4) != null)
                        return CommandBarModeKind.InfoBar;
                    DependencyObject dependencyObject4 = focusedElement;
                    DependencyObject Func5(DependencyObject e) => e.GetVisualOrLogicalParent();
                    Func<DependencyObject, bool> ancestorSelector2 = IsCommandModeSearchControl;
                    if (dependencyObject4.FindAncestorOrSelf(Func5, ancestorSelector2) != null)
                        return CommandBarModeKind.SearchControl;
                    DependencyObject dependencyObject5 = focusedElement;
                    DependencyObject Func7(DependencyObject e) => e.GetVisualOrLogicalParent();
                    Func<DependencyObject, bool> func8 = CommandBarNavigationHelper.GetIsCommandNavigable;
                    if (dependencyObject5.FindAncestorOrSelf(Func7, func8) != null)
                        return CommandBarModeKind.NavigableControl;
                }
                return CommandBarModeKind.None;
            }
        }

        private static bool IsCommandModeSearchControl(DependencyObject e)
        {
            return e is SearchControl && CommandBarNavigationHelper.GetCommandFocusMode(e) != CommandBarNavigationHelper.CommandFocusMode.None;
        }

        internal static bool IsInCommandMode => (uint) CommandModeKind > 0U;

        public static void RegisterMainToolBarTray(ToolBarTray tray)
        {
            MainToolBarTrays.Add(tray);
        }

        public static void RegisterMenu(Controls.Menu.Menu menu)
        {
            MainMenus.Add(menu);
        }

        public static void ResetState()
        {
            CommandState.ResetState();
            CommandBarNavigationState.ResetState();
        }

        public static bool TranslateAccelerator(Key key, bool isKeyUpMessage)
        {
            var modifierKeys = NativeMethods.ModifierKeys;

            var isRepeat = !IsMenuKey(key) && key == _lastKeyDown;
            _lastKeyDown = key;

            bool flag;
            if (!isKeyUpMessage)
            {
                flag = CommandState.FilterKeyDownMessage(key, modifierKeys, isRepeat);
                if (!flag)
                    flag = CommandBarNavigationState.FilterKeyDownMessage(key, modifierKeys, isRepeat);
            }
            else
            {
                flag = CommandState.FilterKeyUpMessage(key);
                if (!flag)
                    flag = CommandBarNavigationState.FilterKeyUpMessage(key);
            }
            return flag;
        }

        private static bool IsMenuKey(Key key)
        {
            if (key == Key.RightAlt || key == Key.LeftAlt)
                return true;
            return false;
        }

        public static void UnregisterMainToolBarTray(ToolBarTray tray)
        {
            MainToolBarTrays.Remove(tray);
        }

        private static List<UIElement> GetSortedNavigationList()
        {
            var uiElementList = MainMenus.Cast<UIElement>().ToList();
            foreach (var mainToolBarTray in MainToolBarTrays)
                uiElementList.AddRange(GetSortedToolBars(mainToolBarTray));
            uiElementList.AddRange(CommandBarNavigationHelper.GetSortedNavigableControls());
            return uiElementList;
        }

        private static IEnumerable<ToolBar> GetSortedToolBars(ToolBarTray tray)
        {
            var toolBarList = new List<ToolBar>(tray.ToolBars);
            toolBarList.Sort((tb1, tb2) =>
                tb1.Band == tb2.Band ? tb1.BandIndex.CompareTo(tb2.BandIndex) : tb1.Band.CompareTo(tb2.Band));
            return toolBarList;
        }

        private static bool NavigateCommandFocus(CommandBarNavigationDirection barNavigationDirection)
        {
            var sortedNavigationList = GetSortedNavigationList();
            var index = sortedNavigationList.FindIndex(control => control.IsKeyboardFocusWithin);
            if (index < 0)
                return false;
            var direction = barNavigationDirection == CommandBarNavigationDirection.Next ? 1 : -1;
            for (var currentIndex = NextIndex(index, direction, sortedNavigationList.Count);
                currentIndex != index;
                currentIndex = NextIndex(currentIndex, direction, sortedNavigationList.Count))
                if (sortedNavigationList[currentIndex] is ItemsControl control)
                    if (TryEnterCommandBar(control))
                        return true;
                    else if (TryFocusElementOrDescendant(sortedNavigationList[currentIndex]))
                        return true;
            return false;
        }

        private static int NextIndex(int currentIndex, int direction, int length)
        {
            return (currentIndex + length + direction) % length;
        }

        private static bool TryEnterCommandBar(ItemsControl control)
        {
            if (control is Menu menu)
                return TryEnterItemsControl<MenuItem>(menu);
            if (control is ToolBar toolBar)
                return TryEnterItemsControl<Control>(toolBar);
            return false;
        }

        private static bool TryEnterItemsControl<TItem>(ItemsControl itemsControl) where TItem : UIElement
        {
            for (var i = 0; i < itemsControl.Items.Count; ++i)
                if (TryFocusItem<TItem>(itemsControl, i))
                    return true;
            return false;
        }

        private static bool TryEnterMenuMode()
        {
            return MainMenus.Any(TryEnterItemsControl<Controls.Menu.MenuItem>);
        }

        private static bool TryEnterToolBarMode()
        {
            return false;
        }

        private static bool TryFocusElementOrDescendant(UIElement element)
        {
            element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            return element.IsKeyboardFocusWithin;
        }

        private static bool TryFocusItem<TItem>(ItemsControl itemsControl, int i) where TItem : UIElement
        {
            if (!(itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is TItem obj) ||
                itemsControl.Items[i] is Separator)
                return false;
            obj.Focus();
            return obj.IsKeyboardFocusWithin;
        }
    }
}