using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IEditorProvider
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        //IDocument Create(Type editorType);
        IEditor Get(Guid editorId);

        bool Handles(string path);

        //Task New(IStorableDocument document, string name);

        void New(NewFileArguments arguments);

        Task Open(IStorableDocument document, string path);
    }
}
