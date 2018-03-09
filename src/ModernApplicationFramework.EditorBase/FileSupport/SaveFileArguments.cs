using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class SaveFileArguments
    {
        public string FileName { get; }

        public ISupportedFileDefinition FileDefinition { get; }

        public string FullFilePath { get; }

        public SaveFileArguments(string fullFilePath, string fileName, ISupportedFileDefinition fileDefinition)
        {
            FullFilePath = fullFilePath;
            FileName = fileName;
            FileDefinition = fileDefinition;
        }
    }
}
