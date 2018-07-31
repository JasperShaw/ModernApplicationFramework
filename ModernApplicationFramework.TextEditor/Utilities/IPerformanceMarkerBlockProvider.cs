using System;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    public interface IPerformanceMarkerBlockProvider
    {
        IDisposable CreateBlock(string blockName);
    }
}