using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.Interfaces.Layout;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IEditorProvider
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        //IDocument Create(Type editorType);
        IEditor Create(Type editorType);

        bool Handles(string path);

        //Task New(IStorableDocument document, string name);

        Task New(IStorableEditor editor, string name);

        Task Open(IStorableDocument document, string path);
    }
}
