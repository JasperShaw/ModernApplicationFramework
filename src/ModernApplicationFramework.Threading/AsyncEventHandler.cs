using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    public delegate Task AsyncEventHandler<T>(object sender, T args) where T : EventArgs;
}
