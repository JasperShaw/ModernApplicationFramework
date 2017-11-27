namespace ModernApplicationFramework.EditorBase
{
    public class FileType
    {
        public FileType(string name, string fileExtension)
        {
            Name = name;
            FileExtension = fileExtension;
        }

        public FileType() {}

        public string FileExtension { get; set; }
        public string Name { get; set; }
    }
}