using System;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class OpenFileArguments
    {
        public OpenFileArguments(string path, Guid choosenEditor = default(Guid))
        {
            Path = path;
            Editor = choosenEditor;
        }

        public string Path { get; }

        public Guid Editor { get; }
    }
}