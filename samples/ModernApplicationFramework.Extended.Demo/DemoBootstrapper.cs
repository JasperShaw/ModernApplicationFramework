using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Extended.Demo
{
    public sealed class DemoBootstrapper : ExtendedBootstrapper
    {
        protected override IExtendedEnvironmentVariables EnvironmentVariables => new DemoEnvironmentVariables();

        public DemoBootstrapper()
        {
            Initialize();
        }
    }
}
