using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading.WaitDialog
{
    public class WaitDialogServiceProvider : DisposableObject
    {
        private static WaitDialogServiceProvider _instace;

        public IWaitDialogService Service { get; private set; }

        private WaitDialogServiceProvider(ICancelHandler handler)
        {
            Service = new WaitDialogService(handler);
        }

        public static WaitDialogServiceProvider CreateServiceProvider(ICancelHandler hanlder)
        {
            if (_instace != null)
                return _instace;
            _instace = new WaitDialogServiceProvider(hanlder);
            return _instace;
        }

        protected override void DisposeManagedResources()
        {
            (Service as WaitDialogService)?.Dispose();
            Service = null;
            base.DisposeManagedResources();
        }
    }
}
