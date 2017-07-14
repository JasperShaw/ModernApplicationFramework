using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using ModernApplicationFramework.Controls.Buttons;

namespace ModernApplicationFramework.Controls.AutomationPeer
{
    public class SplitButtonItemAutomationPeer : FrameworkElementAutomationPeer, ISelectionItemProvider, IScrollItemProvider
    {
        private SplitButton _owningButton;

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                if (OwningButton.IsSubmenuOpen)
                    return ((ListBoxItem)Owner).IsSelected;
                return false;
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer => ProviderFromPeer(CreatePeerForElement(OwningButton));

        private SplitButton OwningButton => _owningButton ?? (_owningButton = (SplitButton) ItemsControl.ItemsControlFromItemContainer(Owner));

        public SplitButtonItemAutomationPeer(SplitButtonItem item) : base(item)
        {
            
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
                return this;
            if (patternInterface == PatternInterface.ScrollItem)
                return this;
            return base.GetPattern(patternInterface);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        void ISelectionItemProvider.AddToSelection()
        {
            if (!OwningButton.IsSubmenuOpen)
                return;
            int num = OwningButton.ItemContainerGenerator.IndexFromContainer(Owner);
            if (OwningButton.SelectedIndex >= num)
                return;
            OwningButton.SelectedIndex = num;
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            if (!OwningButton.IsSubmenuOpen)
                return;
            int num = OwningButton.ItemContainerGenerator.IndexFromContainer(Owner);
            if (OwningButton.SelectedIndex < num || num == 0)
                return;
            --OwningButton.SelectedIndex;
        }

        void ISelectionItemProvider.Select()
        {
            if (!OwningButton.IsSubmenuOpen)
                return;
            OwningButton.SelectedIndex = OwningButton.ItemContainerGenerator.IndexFromContainer(Owner);
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            ((FrameworkElement)Owner).BringIntoView();
        }
    }
}
