using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Extended.Annotations;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    internal class ChooseItemsDataSource : INotifyPropertyChanged, IDisposable
    {
        private Guid _activePageGuid;

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

        internal  void AddControlledPages()
        {
            if (ItemDiscoveryService.Instance == null)
                return;
            foreach (var itemType in ItemDiscoveryService.Instance.ItemTypes)
            {
                var pageDataSource = new ToolboxControlledPageDataSource(itemType, this);
                pageDataSource.PropertyChanged += PageDataSourceOnPropertyChanged;
                ControlledPages.Add(pageDataSource);
            }
        }

        internal static void ApplyChangesAction(ChooseItemsDataSource dataSource)
        {
            foreach (var page in dataSource.ControlledPages)
                dataSource.ClientInfoLoader.ApplyChanges(page);
        }

        internal static void SyncToToolboxAction(ChooseItemsDataSource dataSource)
        {
            foreach (var page in dataSource.ControlledPages)
            {
                dataSource.ClientInfoLoader.SyncToToolbox(page);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        //public event EventHandler<ToolboxControlledPageDataSource> PagePropertyChanged; 

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void PageDataSourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        //protected virtual void OnPagePropertyChanged(ToolboxControlledPageDataSource e)
        //{
        //    PagePropertyChanged?.Invoke(this, e);
        //}
    }
}
