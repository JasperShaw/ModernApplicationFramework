using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal interface IInternalTextBufferFactory
    {
        ITextBuffer CreateTextBuffer(string text, IContentType contentType);

        ITextBuffer CreateTextBuffer(string text, IContentType contentType, bool spurnGroup);

        IContentType TextContentType { get; }

        IContentType InertContentType { get; }

        IContentType ProjectionContentType { get; }
    }
}