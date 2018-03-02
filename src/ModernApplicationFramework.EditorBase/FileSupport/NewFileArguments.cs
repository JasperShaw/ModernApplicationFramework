using System;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class NewFileArguments
    {
        public NewFileArguments(ISupportedFileDefinition fileDefinition, string name, Guid choosenEditor = default(Guid))
        {
            FileDefinition = fileDefinition ?? throw new ArgumentNullException();
            FileName = name;
            Editor = choosenEditor == Guid.Empty ? fileDefinition.PreferredEditor : choosenEditor;

        }

        public ISupportedFileDefinition FileDefinition { get; }

        public string FileName { get; }

        public Guid Editor { get; }
    }
}