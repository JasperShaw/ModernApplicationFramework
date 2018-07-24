namespace ModernApplicationFramework.TextEditor
{
    internal class TextDataModel : ITextDataModel
    {
        public ITextBuffer DocumentBuffer { get; }
        public ITextBuffer DataBuffer { get; }

        public TextDataModel(ITextBuffer documentBuffer, ITextBuffer dataBuffer)
        {
            DocumentBuffer = documentBuffer;
            DataBuffer = dataBuffer;


        }
    }
}
