using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.ViewModels;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Core
{
    [Export(typeof(ISupportedFileDefinition))]
    public class TextFileDefinition : ISupportedFileDefinition
    {
        public string Name => "Text File";
        public int SortOrder => 1;
        public string Description => "Open a plain text file";
        public Uri IconSource { get; }
        public FileType FileType => new FileType("TextFile", ".txt");
        public Type PrefferedEditor => typeof(SimpleTextEditorViewModel);
    }
}
