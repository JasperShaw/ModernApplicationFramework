using System;

namespace ModernApplicationFramework.TextEditor
{
    internal class VacuousTextDataModel : ITextDataModel
    {
        public ITextBuffer DocumentBuffer { get; }
        public ITextBuffer DataBuffer => DocumentBuffer;

        public VacuousTextDataModel(ITextBuffer documentBuffer)
        {
            DocumentBuffer = documentBuffer ?? throw new ArgumentNullException(nameof(documentBuffer));

        }
    }
}
