using System;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    public interface ISelectionPainter : IDisposable
    {
        void Activate();
        void Clear();

        void Update(bool selectionChanged);
    }
}