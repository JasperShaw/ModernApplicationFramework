using System;

namespace ModernApplicationFramework.MVVM.Core
{
    public class NewFileCommandArguments
    {
        public NewFileCommandArguments(string name, string extension, Type editor)
        {
            FileName = name;
            FileExtension = extension;
            PreferredEditor = editor;
        }

        public string FileExtension { get; }
        public string FileName { get; }

        public Type PreferredEditor { get; }
    }
}