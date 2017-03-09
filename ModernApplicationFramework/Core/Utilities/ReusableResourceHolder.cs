using System;

namespace ModernApplicationFramework.Core.Utilities
{
    public struct ReusableResourceHolder<TResource> : IDisposable where TResource : class
    {
        private readonly ReusableResourceStoreBase<TResource> _store;
        private TResource _resource;

        public TResource Resource => _resource;

        internal ReusableResourceHolder(ReusableResourceStoreBase<TResource> store, TResource value)
        {
            _store = store;
            _resource = value;
        }

        public void Dispose()
        {
            _store.ReleaseCore(_resource);
            _resource = default(TResource);
        }
    }
}
