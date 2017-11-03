using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar.Utilities
{
    internal sealed class EnumerableSnapshot : DisposableObject, IEnumerable
    {
        private readonly IList _stableList;
        private INotifyCollectionChanged _changeSource;
        private bool _detectedChange;

        public EnumerableSnapshot(IEnumerable source)
        {
            _stableList = new List<object>(source.Cast<object>());
            ChangeSource = source as INotifyCollectionChanged;
        }

        public bool DetectedChange => _detectedChange;

        private INotifyCollectionChanged ChangeSource
        {
            set
            {
                if (_changeSource == value)
                    return;
                if (_changeSource != null)
                    _changeSource.CollectionChanged -= OnCollectionChanged;
                _changeSource = value;
                if (_changeSource == null)
                    return;
                _changeSource.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            _detectedChange = true;
            ChangeSource = null;
        }

        protected override void DisposeManagedResources()
        {
            ChangeSource = null;
        }

        public IEnumerator GetEnumerator()
        {
            return _stableList.GetEnumerator();
        }
    }
}