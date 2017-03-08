using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core.Result
{
    public class ShowWindowResult<TWindow> : OpenResultBase<TWindow>
        where TWindow : IWindow
    {
        private readonly Func<TWindow> _windowLocator = () => IoC.Get<TWindow>();

        [Import]
        public IWindowManager WindowManager { get; set; }

        public ShowWindowResult()
        {

        }

        public ShowWindowResult(TWindow window)
        {
            _windowLocator = () => window;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var window = _windowLocator();

            SetData?.Invoke(window);

            OnConfigure?.Invoke(window);

            window.Deactivated += (s, e) =>
            {
                if (!e.WasClosed)
                    return;

                OnShutDown?.Invoke(window);

                OnCompleted(null, false);
            };

            WindowManager.ShowWindow(window);
        }
    }
}
