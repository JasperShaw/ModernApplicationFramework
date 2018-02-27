using System;
using ModernApplicationFramework.EditorBase.Interfaces;

namespace ModernApplicationFramework.EditorBase.Commands
{
    public class NewFileCommandArguments
    {
        public NewFileCommandArguments(ISupportedFileDefinition fileDefinition, string name, Type choosenEditor = null)
        {
            FileDefinition = fileDefinition ?? throw new ArgumentNullException();
            FileName = name;    
            if (choosenEditor == null)
                Editor = fileDefinition.PreferredEditor;

        }

        public ISupportedFileDefinition FileDefinition { get; }

        public string FileName { get; }

        public Type Editor { get; }
    }
}