using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JetBrains.Annotations;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Core.CommandArguments;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Controls
{

    public partial class FileExtensionItemPresenter : IExtensionDialogItemPresenter, INotifyPropertyChanged
    {
        private IEnumerable<IExtensionDefinition> _itemSource;

        private EventHandler<ItemDoubleClickedEventArgs> _itemDoubleClicked;


        public event EventHandler<ItemDoubleClickedEventArgs> ItemDoubledClicked
        {
            add
            {
                var eventHandler = _itemDoubleClicked;
                EventHandler<ItemDoubleClickedEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler =
                        Interlocked.CompareExchange(ref _itemDoubleClicked,
                            (EventHandler<ItemDoubleClickedEventArgs>) Delegate.Combine(comparand, value), comparand);
                } while (eventHandler != comparand);
            }
            remove
            {
                var eventHandler = _itemDoubleClicked;
                EventHandler<ItemDoubleClickedEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange(ref _itemDoubleClicked,
                        (EventHandler<ItemDoubleClickedEventArgs>) Delegate.Remove(comparand, value), comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public FileExtensionItemPresenter()
        {
            InitializeComponent();
        }

        public ObservableCollection<object> MyObjectCollection { get; set; }

        public IExtensionDefinition SelectedItem { get; private set; }

        public event EventHandler<ItemsChangedEventArgs> OnSelectedItemChanged;

        protected virtual void OnRaiseSelectedItemChanged(ItemsChangedEventArgs e)
        {
            var handler = OnSelectedItemChanged;
            handler?.Invoke(this, e);
        }

        public IEnumerable<IExtensionDefinition> ItemSource
        {
            get { return _itemSource; }
            set
            {
                if (Equals(_itemSource, value))
                    return;
                _itemSource = value;
                OnPropertyChanged();
                ListView.SelectedIndex = 0;
            }
        }

        public bool UsesNameProperty => true;
        public bool UsesPathProperty => false;

        public object CreateResult(string name, string path)
        {
            var fileArgument = SelectedItem as ISupportedFileDefinition;
            return fileArgument == null
                ? null
                : new NewFileCommandArguments(name, fileArgument.FileType.FileExtension, fileArgument.PreferredEditor);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void _listView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeColumn();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeColumn();
        }

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = ListView.SelectedItem as IExtensionDefinition;
            OnRaiseSelectedItemChanged(null);
        }

        private void ResizeColumn()
        {
            var gridView = ListView.View as GridView;
            if (gridView?.Columns == null)
                return;
            gridView.Columns[0].Width = ListView.ActualWidth - 25.0;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<IExtensionDefinition> newList;
            if (ItemSource == null)
                return;
            switch (SortComboBox.SelectedIndex)
            {
                case 0:
                    newList = ItemSource.OrderBy(x => x.SortOrder);
                    break;
                case 1:
                    newList = ItemSource.OrderBy(x => x.Name);
                    break;
                default:
                    newList = ItemSource.OrderByDescending(x => x.Name);
                    break;
            }
            ItemSource = null;
            ItemSource = newList;
        }

        private void ListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || ListView.SelectedItems.Count == 0 || _itemDoubleClicked == null)
                return;
            _itemDoubleClicked(this, new ItemDoubleClickedEventArgs(SelectedItem));
        }
    }

    public class AbstractControlDescriptionProvider<TAbstract, TBase> : TypeDescriptionProvider
    {
        public AbstractControlDescriptionProvider()
            : base(TypeDescriptor.GetProvider(typeof(TAbstract)))
        {
        }

        public override Type GetReflectionType(Type objectType, object instance)
        {
            if (objectType == typeof(TAbstract))
                return typeof(TBase);

            return base.GetReflectionType(objectType, instance);
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == typeof(TAbstract))
                objectType = typeof(TBase);

            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }

}