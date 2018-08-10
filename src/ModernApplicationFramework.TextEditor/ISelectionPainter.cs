using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ISelectionPainter : IDisposable
    {
        void Clear();

        void Activate();

        void Update(bool selectionChanged);
    }
}