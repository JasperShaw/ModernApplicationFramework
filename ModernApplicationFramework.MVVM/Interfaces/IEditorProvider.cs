using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IEditorProvider
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        bool Handles(string path);

        IDocument Create(Type editorType);

        Task New(IStorableDocument document, string name);
        Task Open(IStorableDocument document, string path);
    }
}
