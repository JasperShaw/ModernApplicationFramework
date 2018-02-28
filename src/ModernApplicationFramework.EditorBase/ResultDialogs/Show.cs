using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.ResultDialogs
{
    public static class Show
    {
        public static OpenDocumentResult Document(IDocument document)
        {
            return new OpenDocumentResult(document);
        }

        public static OpenDocumentResult Document(string path)
        {
            return new OpenDocumentResult(path);
        }
    }
}
