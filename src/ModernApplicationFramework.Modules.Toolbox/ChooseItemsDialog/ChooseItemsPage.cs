using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public abstract class ChooseItemsPage : Screen, IToolboxItemPage
    {
        private ChooseItemsPageView _pageView;
        private IItemInfo _caretItem;

        public IObservableCollection<IItemInfo> Items { get; }

        public IItemInfo CaretItem
        {
            get => _caretItem;
            set
            {
                if (Equals(value, _caretItem))
                    return;
                _caretItem = value;
                NotifyOfPropertyChange();
                FocusCaretItem();
            }
        }

        public virtual bool ShowVersion { get; } = true;

        public virtual bool ShowAssembly { get; } = true;

        protected ChooseItemsPage()
        {
            ViewModelBinder.Bind(this, new ChooseItemsPageView(), null);
            Items = new BindableCollection<IItemInfo>();
        }

        public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previousCaretItem = CaretItem;
            if (!(e.OriginalSource is ListView listView))
                return;

            if (e.AddedItems.Count > 0)
            {
                CaretItem = e.AddedItems.Count == 1 || previousCaretItem == null
                    ? (IItemInfo) e.AddedItems[0]
                    : e.AddedItems.Cast<IItemInfo>().OrderBy(item => DistanceBetween(listView, previousCaretItem, item)).Last();
            }
            else
            {
                if (listView.SelectedItems.Count <= 0 || previousCaretItem == null || listView.SelectedItems.Contains(previousCaretItem))
                    return;
                CaretItem = listView.SelectedItems.Cast<IItemInfo>()
                    .OrderBy(item => DistanceBetween(listView, previousCaretItem, item)).Last();
            }

        }

        private int DistanceBetween(ItemsControl listView, IItemInfo previousCaretItem, IItemInfo item)
        {
            return Math.Abs(listView.Items.IndexOf(previousCaretItem) - listView.Items.IndexOf(item));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _pageView.AddColumns(GetColumns());
        }

        protected abstract IEnumerable<HeaderInformation> GetColumns();

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //Release for GC
            _pageView = null;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            //Prevents View from getting destroyed by GC
            if (view is ChooseItemsPageView pageView)
                _pageView = pageView;
        }

        private void FocusCaretItem()
        {
            _pageView.ListView.ScrollIntoView(CaretItem);
            var item = (ListViewItem)_pageView.ListView.ItemContainerGenerator.ContainerFromItem(CaretItem);
            if (item != null)
                item.Focus();
            else
                _pageView.ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (_pageView.ListView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;
            _pageView.ListView.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            FocusCaretItem();
        }
    }

    public interface IItemInfo : INotifyPropertyChanged
    {
        bool IsChecked { get; set; }

        string Name { get; }

        string Version { get; }

        string Assembly { get; }
    }

    public class HeaderInformation
    {
        public string Text { get; }
        public string Name { get; }

        public HeaderInformation(string text, string name)
        {
            Text = text;
            Name = name;
        }
    }
}
