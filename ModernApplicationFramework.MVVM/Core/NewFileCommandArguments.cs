using System;

namespace ModernApplicationFramework.MVVM.Core
{
    public class NewFileCommandArguments
    {
        public string FileName { get; }

        public string FileExtension { get; }

        public Type PreferredEditor { get; }

        public NewFileCommandArguments(string name, string extension, Type editor)
        {
            FileName = name;
            FileExtension = extension;
            PreferredEditor = editor;
        }
    }
}
