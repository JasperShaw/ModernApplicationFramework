using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IEditorProvider
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        IDocument Create(Type editorType);

        bool Handles(string path);

        Task New(IStorableDocument document, string name);
        Task Open(IStorableDocument document, string path);
    }
}