using ModernApplicationFramework.Extended;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo
{
    public class DemoBootstrapper : ExtendedBootstrapper
    {
        protected override IExtendedEnvironmentVariables EnvironmentVariables => new DemoEnvironmentVariables();
    }
}
