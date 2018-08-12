using System;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    public interface ISelectionPainter : IDisposable
    {
        void Clear();

        void Activate();

        void Update(bool selectionChanged);
    }
}