using System;

namespace ModernApplicationFramework.Core.Themes
{
    public abstract class Theme
    {
        public abstract string Name { get; }
        public abstract Uri GetResourceUri();
    }
}