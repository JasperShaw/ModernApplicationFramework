using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core.Result
{
    public class ShowDialogResult<TWindow> : OpenResultBase<TWindow>
        where TWindow : IWindow
    {
        private readonly Func<TWindow> _windowLocator = () => IoC.Get<TWindow>();

        public ShowDialogResult()
        {
        }

        public ShowDialogResult(TWindow window)
        {
            _windowLocator = () => window;
        }

        [Import]
        public IWindowManager WindowManager { get; set; }

        public override void Execute(CoroutineExecutionContext context)
        {
            TWindow window = _windowLocator();

            SetData?.Invoke(window);

            OnConfigure?.Invoke(window);

            bool result = WindowManager.ShowDialog(window).GetValueOrDefault();

            OnShutDown?.Invoke(window);

            OnCompleted(null, !result);
        }
    }
}
