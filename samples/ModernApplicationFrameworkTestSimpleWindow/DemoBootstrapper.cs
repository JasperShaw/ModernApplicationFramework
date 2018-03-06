using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services;

namespace ModernApplicationFrameworkTestSimpleWindow
{
    public sealed class DemoBootstrapper : Bootstrapper
    {
        public DemoBootstrapper() : base(false)
        {
            Initialize();

            var baseGetLog = LogManager.GetLog;

            LogManager.GetLog = t => t == typeof(ViewModelBinder) ? new DebugLog(t) : baseGetLog(t);
        }
    }
}
