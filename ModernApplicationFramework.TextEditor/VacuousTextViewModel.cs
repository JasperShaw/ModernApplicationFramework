using System;

namespace ModernApplicationFramework.TextEditor
{
    internal class VacuousTextViewModel : ITextViewModel
    {
        public ITextDataModel DataModel { get; }
        public ITextBuffer DataBuffer => DataModel.DataBuffer;
        public ITextBuffer EditBuffer { get; }
        public ITextBuffer VisualBuffer => EditBuffer;

        public VacuousTextViewModel(ITextDataModel dataModel) : this(dataModel, dataModel.DataBuffer)
        {
            
        }

        public VacuousTextViewModel(ITextDataModel dataModel, ITextBuffer editBuffer)
        {
            DataModel = dataModel;
            EditBuffer = editBuffer;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
