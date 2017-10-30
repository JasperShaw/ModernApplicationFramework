using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Extended.DemoMin
{
    public class DemoBootstrapper : ExtendedBootstrapper
    {
        protected override IExtendedEnvironmentVariables EnvironmentVariables => new DemoEnvironmentVariables();
    }
}
