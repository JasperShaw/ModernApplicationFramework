namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class SaveFileDialogOptions
    {
        public string Filter { get; set; }

        public int FilterIndex { get; set; } = 1;

        public string InitialDirectory { get; set; }

        public string Title { get; set; }

        public SaveFileDialogFlags Options { get; set; }

        public string FileName { get; set; }
    }
}