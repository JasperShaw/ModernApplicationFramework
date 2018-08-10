using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Utilities
{
    public abstract class AbstractUiThreadOperationContext : IUiThreadOperationContext
    {
        private readonly string _defaultDescription;
        private bool _allowCancellation;
        private int _completedItems;
        private PropertyCollection _properties;
        private IList<IUiThreadOperationScope> _scopes;
        private int _totalItems;

        public virtual bool AllowCancellation
        {
            get
            {
                if (!_allowCancellation)
                    return false;
                if (_scopes == null || _scopes.Count == 0)
                    return _allowCancellation;
                return _scopes.All(s => s.AllowCancellation);
            }
        }

        public virtual string Description
        {
            get
            {
                if (_scopes == null || _scopes.Count == 0)
                    return _defaultDescription;
                if (_scopes.Count == 1)
                    return _scopes[0].Description;
                return string.Join(Environment.NewLine, _scopes.Select(s => s.Description));
            }
        }

        public virtual PropertyCollection Properties => _properties ?? (_properties = new PropertyCollection());

        public virtual IEnumerable<IUiThreadOperationScope> Scopes => LazyScopes;

        public virtual CancellationToken UserCancellationToken => CancellationToken.None;

        protected int CompletedItems => _completedItems;

        protected int TotalItems => _totalItems;

        private IList<IUiThreadOperationScope> LazyScopes => _scopes ?? (_scopes = new List<IUiThreadOperationScope>());

        protected AbstractUiThreadOperationContext(bool allowCancellation, string defaultDescription)
        {
            var str = defaultDescription;
            _defaultDescription = str ?? throw new ArgumentNullException(nameof(defaultDescription));
            _allowCancellation = allowCancellation;
        }

        public virtual IUiThreadOperationScope AddScope(bool allowCancellation, string description)
        {
            var threadOperationScope =
                new UiThreadOperationScope(allowCancellation, description, this);
            LazyScopes.Add(threadOperationScope);
            OnScopesChanged();
            return threadOperationScope;
        }

        public virtual void Dispose()
        {
        }

        public virtual void TakeOwnership()
        {
        }

        protected virtual void OnScopeChanged(IUiThreadOperationScope uiThreadOperationScope)
        {
        }

        protected virtual void OnScopeDisposed(IUiThreadOperationScope scope)
        {
            _allowCancellation &= scope.AllowCancellation;
            _scopes.Remove(scope);
            OnScopesChanged();
        }

        protected virtual void OnScopeProgressChanged(IUiThreadOperationScope changedScope)
        {
            var num1 = 0;
            var num2 = 0;
            foreach (UiThreadOperationScope lazyScope in
                LazyScopes)
            {
                num1 += lazyScope.CompletedItems;
                num2 += lazyScope.TotalItems;
            }

            Interlocked.Exchange(ref _completedItems, num1);
            Interlocked.Exchange(ref _totalItems, num2);
        }

        protected virtual void OnScopesChanged()
        {
        }

        private class UiThreadOperationScope : IUiThreadOperationScope
        {
            private readonly AbstractUiThreadOperationContext _context;
            private bool _allowCancellation;
            private int _completedItems;
            private string _description;
            private IProgress<ProgressInfo> _progress;
            private int _totalItems;

            public bool AllowCancellation
            {
                get => _allowCancellation;
                set
                {
                    if (_allowCancellation == value)
                        return;
                    _allowCancellation = value;
                    _context.OnScopeChanged(this);
                }
            }

            public int CompletedItems => _completedItems;

            public IUiThreadOperationContext Context => _context;

            public string Description
            {
                get => _description;
                set
                {
                    if (_description == value)
                        return;
                    _description = value;
                    _context.OnScopeChanged(this);
                }
            }

            public IProgress<ProgressInfo> Progress => _progress ?? (_progress =
                                                           new Progress<ProgressInfo>(
                                                               OnProgressChanged));

            public int TotalItems => _totalItems;

            public UiThreadOperationScope(bool allowCancellation, string description,
                AbstractUiThreadOperationContext context)
            {
                var operationContext = context;
                _context = operationContext ?? throw new ArgumentNullException(nameof(context));
                AllowCancellation = allowCancellation;
                Description = description ?? "";
            }

            public void Dispose()
            {
                _context.OnScopeDisposed(this);
            }

            private void OnProgressChanged(ProgressInfo progressInfo)
            {
                Interlocked.Exchange(ref _completedItems, progressInfo.CompletedItems);
                Interlocked.Exchange(ref _totalItems, progressInfo.TotalItems);
                _context.OnScopeProgressChanged(this);
            }
        }
    }
}