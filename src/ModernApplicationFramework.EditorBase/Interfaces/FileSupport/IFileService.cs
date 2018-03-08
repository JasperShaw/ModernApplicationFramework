using System.Collections.Generic;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IFileService
    {
        IReadOnlyList<IFile> OpenedFiles { get; }

        IFile GetOpenedFile(string fileName);

        IFile CreateFile(NewFileArguments arguments);

        IFile OpenExistingFile(OpenFileArguments arguments);

        bool IsFileOpen(string filePath, out IEditor editor);

        IReadOnlyCollection<OpenFileArguments> ShowOpenFilesDialog();
    }
}