using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Modules.Editor.Utilities
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