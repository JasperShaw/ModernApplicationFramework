using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewModel : IDisposable
    {
        ITextDataModel DataModel { get; }

        ITextBuffer DataBuffer { get; }

        ITextBuffer EditBuffer { get; }

        ITextBuffer VisualBuffer { get; }
    }
}
