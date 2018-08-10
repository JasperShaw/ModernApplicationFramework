using System;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IUrlTag : ITag
    {
        Uri Url { get; }
    }
}