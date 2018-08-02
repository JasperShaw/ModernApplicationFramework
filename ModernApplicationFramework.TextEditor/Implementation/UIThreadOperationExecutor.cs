using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(IUiThreadOperationExecutor))]
    internal class UiThreadOperationExecutor : IUiThreadOperationExecutor
    {
        [Import]
        private IEnumerable<Lazy<IUiThreadOperationExecutor, IOrderable>> _unorderedImplementations;
        private IUiThreadOperationExecutor _bestImpl;

        private IUiThreadOperationExecutor BestImplementation
        {
            get
            {
                if (_bestImpl == null)
                {
                    IList<Lazy<IUiThreadOperationExecutor, IOrderable>> lazyList = Orderer.Order(_unorderedImplementations);
                    if (lazyList.Count == 0)
                        throw new ImportCardinalityMismatchException(
                            $"Expected to import at least one export of {typeof(IUiThreadOperationExecutor).FullName}, but got none.");
                    _bestImpl = lazyList[0].Value;
                }
                return _bestImpl;
            }
        }

        public IUiThreadOperationContext BeginExecute(string title, string defaultDescription, bool allowCancellation, bool showProgress)
        {
            return BestImplementation.BeginExecute(title, defaultDescription, allowCancellation, showProgress);
        }

        public UiThreadOperationStatus Execute(string title, string defaultDescription, bool allowCancellation, bool showProgress, Action<IUiThreadOperationContext> action)
        {
            return BestImplementation.Execute(title, defaultDescription, allowCancellation, showProgress, action);
        }
    }
}