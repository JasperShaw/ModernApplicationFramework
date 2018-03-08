namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class SaveFileArguments
    {
        public string FileName { get; }

        public string FullFilePath { get; }

        public SaveFileArguments(string fullFilePath, string fileName)
        {
            FullFilePath = fullFilePath;
            FileName = fileName;
        }
    }
}
