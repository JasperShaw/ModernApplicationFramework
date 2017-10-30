using System;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Controls.ComboBox;

namespace ModernApplicationFramework.Controls.AutomationPeer
{
    public class ComboBoxDataItemAutomationPeer : SelectorItemAutomationPeer, ISelectionItemProvider, IScrollItemProvider
    {
        private ComboBox.ComboBox _parentCombo;

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                var parentCombo = ParentCombo;
                var dataContext = parentCombo?.DataContext as ComboBoxDataSource;
                if (dataContext == null || dataContext.IsDisposed)
                    return false;
                if (parentCombo.VisualDataSource.IsEditable)
                    return dataContext.DisplayedText == Text;
                var itemDataSource = ItemDataSource;
                if (itemDataSource != null && !itemDataSource.IsDisposed)
                    return itemDataSource.Equals(dataContext);
                return false;
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer => ProviderFromPeer(ItemsControlAutomationPeer);

        private ComboBox.ComboBox ParentCombo
        {
            get
            {
                if (_parentCombo != null)
                    return _parentCombo;
                var controlAutomationPeer = ItemsControlAutomationPeer as ComboBoxAutomationPeer;
                if (controlAutomationPeer != null)
                    _parentCombo = controlAutomationPeer.Owner as ComboBox.ComboBox;
                return _parentCombo;
            }
        }

        private ComboBoxDataSource ItemDataSource => Item as ComboBoxDataSource;


        private string Text { get; }

        public ComboBoxDataItemAutomationPeer(object dataSource, SelectorAutomationPeer selectorAutomationPeer) : base(dataSource, selectorAutomationPeer)
        {
            if (selectorAutomationPeer == null)
                throw new ArgumentNullException(nameof(selectorAutomationPeer));
            Text = dataSource?.ToString() ?? throw new ArgumentNullException(nameof(dataSource));
        }

        protected override string GetClassNameCore()
        {
            return "ListBoxItem";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.SelectionItem:
                    return this;
                case PatternInterface.ScrollItem:
                    return this;
            }
            return null;
        }

        protected override string GetNameCore()
        {
            return Text;
        }

        public void ScrollIntoView()
        {
            if (_parentCombo == null)
                return;
            if (!_parentCombo.IsDropDownOpen)
                _parentCombo.IsDropDownOpen = true;
            var comboBoxitem = _parentCombo.ItemContainerGenerator.ContainerFromItem(Item) as ComboBoxItem;
            comboBoxitem?.BringIntoView();
        }

        void ISelectionItemProvider.AddToSelection()
        {
            ((ISelectionItemProvider)this).Select();
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
        }

        void ISelectionItemProvider.Select()
        {
            if (((ISelectionItemProvider)this).IsSelected)
                return;
            var parentCombo = ParentCombo;
            if (parentCombo == null)
                return;
            int indexForItem = FindIndexForItem();
            if (indexForItem == -1)
                return;
            parentCombo.HandleComboSelection(false, indexForItem);
        }

        private int FindIndexForItem()
        {
            var parentCombo = ParentCombo;
            if (parentCombo != null)
            {
                var itemDataSource = ItemDataSource;
                if (itemDataSource != null && !itemDataSource.IsDisposed)
                {
                    for (var index = 0; index < parentCombo.Items.Count; ++index)
                    {
                        if (itemDataSource.Equals(parentCombo.Items[index]))
                            return index;
                    }
                }
            }
            return -1;
        }
    }
}
