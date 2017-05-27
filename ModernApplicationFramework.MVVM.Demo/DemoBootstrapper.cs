using ModernApplicationFramework.Extended;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo
{
    public class DemoBootstrapper : ExtendedBootstrapper
    {
        protected override IExtendedEnvironmentVarirables EnvironmentVarirables => new DemoEnvironmentVariables();
    }
}
