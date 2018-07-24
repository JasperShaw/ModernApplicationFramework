using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Utilities.Threading
{
    public delegate Task AsyncEventHandler<in T>(object sender, T args) where T : EventArgs;
}
