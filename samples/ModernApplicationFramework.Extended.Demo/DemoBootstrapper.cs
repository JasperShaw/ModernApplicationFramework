using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Extended.Demo
{
    public class DemoBootstrapper : ExtendedBootstrapper
    {
        protected override IExtendedEnvironmentVariables EnvironmentVariables => new DemoEnvironmentVariables();
    }
}
