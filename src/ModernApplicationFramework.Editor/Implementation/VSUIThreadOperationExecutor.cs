using System;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation
{
    //[ExportImplementation(typeof(IUiThreadOperationExecutor))]
    [Name("UI thread operation executor")]
    [Order(Before = "default")]
    internal sealed class MafUiThreadOperationExecutor : IUiThreadOperationExecutor
    {
        //TODO: Implement Wait window stuff

        //private IVsThreadedWaitDialogFactory _vsThreadedWaitDialogFactory;

        [ImportingConstructor]
        public MafUiThreadOperationExecutor()
        {
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
            //if (this._vsThreadedWaitDialogFactory == null)
            //{
            //    this._vsThreadedWaitDialogFactory = (IVsThreadedWaitDialogFactory)this._serviceProvider.GetService(typeof(SVsThreadedWaitDialogFactory));
            //    if (this._vsThreadedWaitDialogFactory == null)
            //        throw new InvalidOperationException("Cannot get IVsThreadedWaitDialogFactory");
            //}

            return new UiThreadOperationContext(/*_vsThreadedWaitDialogFactory,*/ title,
                defaultDescription, allowCancellation, showProgress);
        }
    }
}