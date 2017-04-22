using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Converters;

namespace ModernApplicationFramework.Controls
{
    //[TemplatePart(Name = PartActionButton, Type = typeof(System.Windows.Controls.Button))]
    //public class SplittButton : DropDownButton
    //{
    //    private const string PartActionButton = "PART_ActionButton";

    //    static SplittButton()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(SplittButton),
    //            new FrameworkPropertyMetadata(typeof(SplittButton)));
    //    }

    //    public override void OnApplyTemplate()
    //    {
    //        base.OnApplyTemplate();
    //        Button = GetTemplateChild(PartActionButton) as System.Windows.Controls.Button;
    //    }
    //}
    public class SplitButton : System.Windows.Controls.MenuItem
    {
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

        //protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        //{
        //    return (System.Windows.Automation.Peers.AutomationPeer)new SplitButtonAutomationPeer(this);
        //}

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
            BindingOperations.SetBinding(element, IsHighlightedProperty, binding1);
        }

        protected override void OnSubmenuClosed(RoutedEventArgs e)
        {
            SelectedIndex = 0;
            base.OnSubmenuClosed(e);
        }

        protected override void OnSubmenuOpened(RoutedEventArgs e)
        {
            base.OnSubmenuOpened(e);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplitButton)d).OnSelectedIndexChanged(e);
        }

        private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {

        }
    }
}