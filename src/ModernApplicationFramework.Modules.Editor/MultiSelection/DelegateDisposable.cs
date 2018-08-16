using System;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class DelegateDisposable : IDisposable
    {
        private readonly Action _onDisposed;

        public DelegateDisposable(Action onDisposed)
        {
            _onDisposed = onDisposed;
        }

        public void Dispose()
        {
            _onDisposed();
        }
    }
}
