using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    [Export(typeof(IUiThreadOperationExecutor))]
    internal class UiThreadOperationExecutor : IUiThreadOperationExecutor
    {
        [ImportImplementations(typeof(IUiThreadOperationExecutor))]
        private IEnumerable<Lazy<IUiThreadOperationExecutor, IOrderable>> _unorderedImplementations;
        private IUiThreadOperationExecutor _bestImpl;

        private IUiThreadOperationExecutor BestImplementation
        {
            get
            {
                if (_bestImpl == null)
                {
                    var lazyList = Orderer.Order(_unorderedImplementations);
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