using System;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class OpenFileArguments
    {
        public ISupportedFileDefinition FileDefinition { get; }

        public string Path { get; }

        public string Name => string.IsNullOrEmpty(Path) ? string.Empty : System.IO.Path.GetFileName(Path);

        public Guid Editor { get; }

        public OpenFileArguments(ISupportedFileDefinition fileDefinition, string path, Guid choosenEditor = default(Guid))
        {
            FileDefinition = fileDefinition;
            Path = path;
            if (fileDefinition == null)
                Editor = Guid.Empty;
            else
                Editor = choosenEditor == Guid.Empty ? fileDefinition.PreferredEditor : choosenEditor;
        }
    }
}