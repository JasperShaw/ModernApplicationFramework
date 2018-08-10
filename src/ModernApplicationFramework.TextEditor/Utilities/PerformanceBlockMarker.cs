using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class PerformanceBlockMarker
    {
        internal IDisposable CreateBlock(string blockName)
        {
            return new Block();
        }

        private class Block : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}