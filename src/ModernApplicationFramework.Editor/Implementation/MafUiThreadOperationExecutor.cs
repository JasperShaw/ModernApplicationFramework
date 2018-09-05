using System;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation
{
    [ExportImplementation(typeof(IUiThreadOperationExecutor))]
    [Name("UI thread operation executor")]
    [Order(Before = "default")]
    internal sealed class MafUiThreadOperationExecutor : IUiThreadOperationExecutor
    {
        private readonly IWaitDialogFactory _waitDialogFactory;

        [ImportingConstructor]
        public MafUiThreadOperationExecutor(IWaitDialogFactory waitDialogFactory)
        {
            _waitDialogFactory = waitDialogFactory;
        }

        public UiThreadOperationStatus Execute(string title, string defaultDescription, bool allowCancellation, bool showProgress, Action<IUiThreadOperationContext> action)
        {
            using (var operationContext = BeginExecute(title, defaultDescription, allowCancellation, showProgress))
            {
                try
                {
                    action(operationContext);
                    return UiThreadOperationStatus.Completed;
                }
                catch (OperationCanceledException)
                {
                    return UiThreadOperationStatus.Canceled;
                }
                catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is OperationCanceledException))
                {
                    return UiThreadOperationStatus.Canceled;
                }
            }
        }

        public IUiThreadOperationContext BeginExecute(string title, string defaultDescription, bool allowCancellation, bool showProgress)
        {

            if (_waitDialogFactory == null)
                throw new InvalidOperationException("Cannot get IVsThreadedWaitDialogFactory");

            return new UiThreadOperationContext(_waitDialogFactory, title,
                defaultDescription, allowCancellation, showProgress);
        }
    }
}