using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.AutomationPeer;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Converters;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc />
    /// <summary>
    /// Custom split button control
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.MenuItem" />
    public class SplitButton : MenuItem
    {
        /// <summary>
        /// The selected item index
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty;
        
        private readonly IValueConverter _itemIsHighlightedConverter;

        static SplitButton()
        {
            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SplitButton),
                new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public double LastYValueOnMouseEnter { get; set; }

        public SplitButton()
        {
            _itemIsHighlightedConverter = new SplitButtonItemHighlightConverter(this);
            LastYValueOnMouseEnter = double.NaN;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (MenuUtilities.HandleKeyDownForToolBarHostedMenuItem(this, e))
                return;
            var flag = false;
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        if (IsSubmenuOpen)
                        {
                            IsSubmenuOpen = false;
                            SelectedIndex = 0;
                            Keyboard.Focus(this);
                            flag = true;
                        }
                        break;
                    case Key.End:
                        Keyboard.Focus(
                            ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as SplitButtonItem);
                        flag = true;
                        break;
                    case Key.Home:
                        Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(0) as SplitButtonItem);
                        flag = true;
                        break;
                    case Key.Up:
                        HandleNavigationRequest(FocusNavigationDirection.Up);
                        flag = true;
                        break;
                    case Key.Down:
                        if (!IsSubmenuOpen)
                        {
                            IsSubmenuOpen = true;
                            Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(0) as SplitButtonItem);
                        }
                        else
                            HandleNavigationRequest(FocusNavigationDirection.Down);
                        flag = true;
                        break;
                }
                if (flag)
                    return;
                base.OnKeyDown(e);
            }
        }

        private void HandleNavigationRequest(FocusNavigationDirection direction)
        {
            var splitButtonItem = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as SplitButtonItem;
            splitButtonItem?.MoveFocus(new TraversalRequest(direction));
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new SplitButtonAutomationPeer(this);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SplitButtonItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var binding1 = new Binding
            {
                Source = this,
                Path = new PropertyPath(SelectedIndexProperty),
                Mode = BindingMode.TwoWay,
                Converter = _itemIsHighlightedConverter,
                ConverterParameter = element
            };
            BindingOperations.SetBinding(element, SplitButtonItem.IsHighlightedProperty, binding1);
        }

        protected override void OnSubmenuClosed(RoutedEventArgs e)
        {
            SelectedIndex = 0;
            base.OnSubmenuClosed(e);
        }

        protected override void OnSubmenuOpened(RoutedEventArgs e)
        {
            UpdateChildCollection();
            base.OnSubmenuOpened(e);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplitButton)d).OnSelectedIndexChanged(e);
        }

        private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!AutomationPeerHelper.SelectionListenersExist())
                return;
            var objectList1 = new List<object>();
            var objectList2 = new List<object>();
            if ((int) e.OldValue < (int) e.NewValue)
            {
                var num = (int) e.OldValue < 0 ? 0 : (int) e.OldValue;
                var newValue = (int) e.NewValue;
                for (var index = num; index <= newValue; ++index)
	                if (HasItems)
		                objectList1.Add(Items[index]);
            }
            else
            {
                var oldValue = (int) e.OldValue;
                var num = (int) e.NewValue < 0 ? 0 : (int) e.NewValue;
                for(var index = oldValue; index >= num; --index)
	                if (HasItems)
						objectList2.Add(Items[index]);
            }
            AutomationPeerHelper.RaiseSelectionEvents(
                AutomationPeerHelper.CreatePeerFromElement<SplitButtonAutomationPeer>(this),
                new SelectionChangedEventArgs(Selector.SelectionChangedEvent, objectList2, objectList1), null,
                this.HasGeneratedContainers(), GetPeer);
        }

        private System.Windows.Automation.Peers.AutomationPeer GetPeer(object o)
        {
            var splitButtonItem = ItemContainerGenerator.ContainerFromItem(o) as SplitButtonItem;
            if (splitButtonItem == null)
                return null;
            return AutomationPeerHelper.CreatePeerFromElement<SplitButtonItemAutomationPeer>(splitButtonItem);
        }

        internal void Invoke()
        {
            var dataContext = (CommandBarDefinitionBase) DataContext;
            
            if (dataContext?.CommandDefinition == null || !(dataContext.CommandDefinition is CommandSplitButtonDefinition splitCommandDefinition))
                return;
            var selectedIndex = SelectedIndex;
            IsSubmenuOpen = false;
            SelectedIndex = 0;
            splitCommandDefinition.Execute(selectedIndex + 1);
            if (System.Windows.Automation.Peers.AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
                AutomationPeerHelper.CreatePeerFromElement<SplitButtonAutomationPeer>(this).RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
            if (!IsKeyboardFocusWithin)
                return;
            Keyboard.Focus(null);
        }

        public void UpdateChildCollection()
        {
            
        }
    }
}