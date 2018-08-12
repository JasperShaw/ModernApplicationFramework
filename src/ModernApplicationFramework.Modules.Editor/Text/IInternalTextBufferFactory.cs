using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal interface IInternalTextBufferFactory
    {
        IContentType InertContentType { get; }

        IContentType ProjectionContentType { get; }

        IContentType TextContentType { get; }
        ITextBuffer CreateTextBuffer(string text, IContentType contentType);

        ITextBuffer CreateTextBuffer(string text, IContentType contentType, bool spurnGroup);
    }
}