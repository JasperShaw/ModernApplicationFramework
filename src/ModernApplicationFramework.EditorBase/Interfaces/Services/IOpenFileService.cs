using System;
using ModernApplicationFramework.EditorBase.FileSupport;

namespace ModernApplicationFramework.EditorBase.Interfaces.Services
{
    public interface IOpenFileService
    {
        void OpenFile(OpenFileArguments args);

        bool TryOpenFile(OpenFileArguments args);

        bool TryOpenFile(string path, Guid editorGuid);

        bool TryOpenFile(string path);

        void OpenStandardEditor(string path);

        void OpenSpecificEditor(string path, Guid editorGuid);
    }
}