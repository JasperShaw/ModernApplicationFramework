using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IUrlTag : ITag
    {
        Uri Url { get; }
    }
}