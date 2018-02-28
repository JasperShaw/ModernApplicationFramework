using System;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.Commands
{
    public class NewFileCommandArguments
    {
        public NewFileCommandArguments(ISupportedFileDefinition fileDefinition, string name, Guid choosenEditor = default(Guid))
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