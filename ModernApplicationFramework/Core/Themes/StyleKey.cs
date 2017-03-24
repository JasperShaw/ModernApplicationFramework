using System.Reflection;
using System.Windows;

namespace ModernApplicationFramework.Core.Themes
{
    public sealed class StyleKey<T> : ResourceKey
    {

        private Assembly assembly;

        public override Assembly Assembly
        {
            get
            {
                var assembly = this.assembly;
                if (assembly != null)
                    return assembly;
                return this.assembly = typeof(T).Assembly;
            }
        }
    }
}
