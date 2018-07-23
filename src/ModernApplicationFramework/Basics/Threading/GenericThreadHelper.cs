using System;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class GenericThreadHelper : ThreadHelper
    {
        protected override IDisposable GetInvocationWrapper()
        {
            return null;
        }
    }
}