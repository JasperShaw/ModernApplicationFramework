using System;

namespace ModernApplicationFramework.Core.Themes
{
    public abstract class Theme
    {
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract Uri GetResourceUri();
    }
}