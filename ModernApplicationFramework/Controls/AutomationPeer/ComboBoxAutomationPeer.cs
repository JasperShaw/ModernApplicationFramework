using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Controls.AutomationPeer
{
    public class ComboBoxAutomationPeer : System.Windows.Automation.Peers.ComboBoxAutomationPeer, ISelectionProvider
    {

        private static readonly IRawElementProviderSimple[] EmptySelection;
        private readonly Dictionary<object, ComboBoxDataItemAutomationPeer> _itemPeers;
        private readonly IRawElementProviderSimple[] _selectedItem;

        internal bool HasHandedOutChildPeers => (uint)_itemPeers.Count > 0U;

        bool ISelectionProvider.CanSelectMultiple => false;

        bool ISelectionProvider.IsSelectionRequired => false;


        static ComboBoxAutomationPeer()
        {
            EmptySelection = new IRawElementProviderSimple[0];
        }

        public ComboBoxAutomationPeer(ComboBox.ComboBox owner) : base(owner)
        {
            _selectedItem = new IRawElementProviderSimple[1];
            _itemPeers = new Dictionary<object, ComboBoxDataItemAutomationPeer>(owner.Items.Count);
            var itemsSource = owner.ItemsSource as INotifyCollectionChanged;
            if (itemsSource == null)
                return;
            itemsSource.CollectionChanged += OnCollectionChanged;
        }

        internal ComboBoxDataItemAutomationPeer GetItemAutomationPeer(object item)
        {
            return (ComboBoxDataItemAutomationPeer) CreateItemAutomationPeer(item);
        }

        internal void RemoveItemFromPeerCache(object item)
        {
            _itemPeers.Remove(item);
            ResetChildrenCache();
        }

        protected override bool HasKeyboardFocusCore()
        {
            return Owner.IsKeyboardFocusWithin;
        }

        internal void RaiseIsSelectedChanged(object item, bool oldValue, bool newValue)
        {
            var itemAutomationPeer = GetItemAutomationPeer(item);
            itemAutomationPeer?.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, Boxes.Box(oldValue), Boxes.Box(newValue));
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            ComboBoxDataItemAutomationPeer itemAutomationPeer;
            if (!_itemPeers.TryGetValue(item, out itemAutomationPeer))
            {
                //var dataSource = item as ComboBoxDataSource;
                //if (dataSource == null)
                //    throw new InvalidOperationException();
                var peerFromElement = AutomationPeerHelper.CreatePeerFromElement<SelectorAutomationPeer>(Owner);
                itemAutomationPeer = new ComboBoxDataItemAutomationPeer(item, peerFromElement);
                _itemPeers.Add(item, itemAutomationPeer);
            }
            return itemAutomationPeer;
        }

        protected override List<System.Windows.Automation.Peers.AutomationPeer> GetChildrenCore()
        {
            var automationPeerList = base.GetChildrenCore();
            var owner = Owner as ComboBox.ComboBox;
            if (owner == null)
                return automationPeerList;
            var dataContext = owner.DataContext as ComboBoxDataSource;
            if (dataContext == null || dataContext.IsDisposed)
                return automationPeerList;
            var displayedItem = dataContext.DisplayedItem;
            if (displayedItem != null && !DisplayedItemIsInChildCollection(displayedItem))
            {
                if (automationPeerList == null)
                    automationPeerList = new List<System.Windows.Automation.Peers.AutomationPeer>(1);
                automationPeerList.Add(CreateItemAutomationPeer(displayedItem));
            }
            return automationPeerList;
        }

        public override object GetPattern(PatternInterface pattern)
        {
            if (pattern == PatternInterface.Selection)
                return this;
            return base.GetPattern(pattern);
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            var owner = Owner as ComboBox.ComboBox;
            var dataContext = owner?.DataContext as ComboBoxDataSource;
            if (dataContext == null || dataContext.IsDisposed)
                return EmptySelection;
            var vsUiDataSource = dataContext.DisplayedItem;
            if (vsUiDataSource == null)
                return EmptySelection;
            _selectedItem[0] = ProviderFromPeer(CreateItemAutomationPeer(vsUiDataSource));
            return _selectedItem;
        }

        private bool DisplayedItemIsInChildCollection(object displayedItem)
        {
            var owner = Owner as ComboBox.ComboBox;
            if (owner == null)
                return false;
            return owner.ItemsSource.Cast<object>().Contains(displayedItem);
        }


        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    var enumerator = e.OldItems.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                            _itemPeers.Remove(enumerator.Current);
                        break;
                    }
                    finally
                    {
                        var disposable = enumerator as IDisposable;
                        disposable?.Dispose();
                    }
                case NotifyCollectionChangedAction.Replace:
                    _itemPeers.Remove(e.OldItems[0]);
                    break;
                    case NotifyCollectionChangedAction.Reset:
                        _itemPeers.Clear();
                    break;
            }
            RaiseAutomationEvent(AutomationEvents.StructureChanged);
        }
    }
}
