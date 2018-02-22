using System;

namespace ModernApplicationFramework.EditorBase.Commands
{
    public class OpenFileWithCommandArguments
    {
        public OpenFileWithCommandArguments(string name, Type editor)
        {
            FullFileName = name;
            Editor = editor;
        }

        public string FullFileName { get; }

        public Type Editor { get; }
    }
}