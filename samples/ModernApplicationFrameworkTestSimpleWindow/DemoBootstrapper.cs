﻿using ModernApplicationFramework.Basics.Services;

namespace ModernApplicationFrameworkTestSimpleWindow
{
    public sealed class DemoBootstrapper : Bootstrapper
    {
        public DemoBootstrapper() : base(false)
        {
            Initialize();
        }
    }
}