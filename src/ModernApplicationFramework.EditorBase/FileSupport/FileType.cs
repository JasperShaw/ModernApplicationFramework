namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileType
    {
        public FileType(string fileExtension)
        {
            FileExtension = fileExtension;
        }

        public FileType() {}

        public string FileExtension { get; set; }
    }
}