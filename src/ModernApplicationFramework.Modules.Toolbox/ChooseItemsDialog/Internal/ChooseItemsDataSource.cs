﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal
{
    internal class ChooseItemsDataSource : INotifyPropertyChanged, IDisposable
    {
        private Guid _activePageGuid;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid ActivePageGuid
        {
            get => _activePageGuid;
            set
            {
                if (value.Equals(_activePageGuid)) return;
                _activePageGuid = value;
                OnPropertyChanged();
            }
        }

        public PackageInfoLoader ClientInfoLoader { get; private set; }

        public IList<ToolboxControlledPageDataSource> ControlledPages { get; }

        public ChooseItemsDataSource()
        {
            ControlledPages = new List<ToolboxControlledPageDataSource>();
            AddControlledPages();
            ClientInfoLoader = new PackageInfoLoader(this);
        }

        public void Dispose()
        {
            if (ClientInfoLoader != null)
            {
                ClientInfoLoader.Dispose();
                ClientInfoLoader = null;
            }

            foreach (var page in ControlledPages)
                page.PropertyChanged -= PageDataSourceOnPropertyChanged;
            ControlledPages.Clear();
        }

        internal static IEnumerable<string> ApplyChangesAction(ChooseItemsDataSource dataSource)
        {
            var addedItems = new List<ItemDataSource>();
            foreach (var page in dataSource.ControlledPages)
                addedItems.AddRange(dataSource.ClientInfoLoader.ApplyChanges(page));

            var service = IoC.Get<IToolboxService>();

            return (from item in addedItems
                select item.Definition
                into defintion
                where !service.FindItemsByDefintion(defintion).Any(x => x.IsVisible)
                select defintion.Name).ToList();
        }


        internal static void SyncToToolboxAction(ChooseItemsDataSource dataSource)
        {
            foreach (var page in dataSource.ControlledPages) dataSource.ClientInfoLoader.SyncToToolbox(page);
        }

        internal void AddControlledPages()
        {
            if (ItemDiscoveryService.Instance == null)
                return;
            foreach (var itemType in ItemDiscoveryService.Instance.ItemTypes)
            {
                var pageDataSource = new ToolboxControlledPageDataSource(itemType);
                pageDataSource.PropertyChanged += PageDataSourceOnPropertyChanged;
                ControlledPages.Add(pageDataSource);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PageDataSourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ItemDataSource itemData)
            {
                if (e.PropertyName == nameof(itemData.IsChecked)) UpdateSameItemsAcrossPages(itemData);
            }
            else
            {
                PropertyChanged?.Invoke(sender, e);
            }
        }

        private void UpdateSameItemsAcrossPages(ItemDataSource itemData)
        {
            var pagesToCheck = ControlledPages.Where(x => x.Guid != ActivePageGuid);

            foreach (var page in pagesToCheck)
            foreach (var item in page.Items)
                if (item.Equals(itemData))
                    item.IsChecked = itemData.IsChecked;
        }
    }
}