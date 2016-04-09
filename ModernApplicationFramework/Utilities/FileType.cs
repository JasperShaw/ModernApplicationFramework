namespace ModernApplicationFramework.Utilities
{
    public class FileType
    {
        public string Name { get; set; }
        public string FileExtension { get; set; }

        public FileType(string name, string fileExtension)
        {
            Name = name;
            FileExtension = fileExtension;
        }

        public FileType()
        {

        }
    }
}
