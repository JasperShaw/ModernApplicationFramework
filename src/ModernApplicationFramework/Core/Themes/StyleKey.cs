using System.Reflection;
using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Core.Themes
{
    /// <inheritdoc />
    /// <summary>
    /// A custom <see cref="T:System.Windows.ResourceKey" /> used by <see cref="IExposeStyleKeys"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="T:System.Windows.ResourceKey" />
    public sealed class StyleKey<T> : ResourceKey
    {
        private Assembly _assembly;

        public override Assembly Assembly
        {
            get
            {
                var assembly = _assembly;
                if (assembly != null)
                    return assembly;
                return _assembly = typeof(T).Assembly;
            }
        }
    }
}
