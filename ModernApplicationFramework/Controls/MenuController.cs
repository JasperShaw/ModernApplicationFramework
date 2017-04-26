using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls
{
    public class MenuController : MenuItem, IExposeStyleKeys
    {
        public static readonly DependencyProperty AnchorItemProperty;

        private static ResourceKey buttonStyleKey;
        private static ResourceKey menuControllerStyleKey;
        private static ResourceKey comboBoxStyleKey;
        private static ResourceKey menuStyleKey;
        private static ResourceKey separatorStyleKey;
        public new static ResourceKey ButtonStyleKey => buttonStyleKey ?? (buttonStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey MenuControllerStyleKey => menuControllerStyleKey ?? (menuControllerStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey ComboBoxStyleKey => comboBoxStyleKey ?? (comboBoxStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey MenuStyleKey => menuStyleKey ?? (menuStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey SeparatorStyleKey => separatorStyleKey ?? (separatorStyleKey = new StyleKey<MenuController>());

        public object AnchorItem
        {
            get => GetValue(AnchorItemProperty);
            set => SetValue(AnchorItemProperty, value);
        }

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        static MenuController()
        {
            AnchorItemProperty = DependencyProperty.Register("AnchorItem", typeof(object), typeof(MenuController),
                new FrameworkPropertyMetadata(null, CoerceAnchorItemCallback));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuController), new FrameworkPropertyMetadata(typeof(MenuController)));
            //EventManager.RegisterClassHandler(typeof(MenuItem), MenuItem.CommandExecutedRoutedEvent, OnCommandExecuted);
        }

        private static object CoerceAnchorItemCallback(DependencyObject d, object basevalue)
        {
            return ((MenuController) d).CoerceAnchorItemCallback(basevalue);
        }

        private object CoerceAnchorItemCallback(object basevalue)
        {
            if (basevalue != null && !(basevalue is object))
                return DependencyProperty.UnsetValue;
            return basevalue;
        }

        public MenuController()
        {
            DteFocusHelper.HookAcquireFocus(this);
        }

        //protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        //{
        //    return (System.Windows.Automation.Peers.AutomationPeer)new VsMenuControllerAutomationPeer(this);
        //}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;

            }
            else
                base.OnKeyDown(e);
        }
    }
}
