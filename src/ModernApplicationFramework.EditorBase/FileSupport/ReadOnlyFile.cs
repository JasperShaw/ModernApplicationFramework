using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public sealed class ReadOnlyFile : MafFile, IReadOnlyFile
    {
        public ReadOnlyFile(string path, string name) : base(path, name)
        {
        }

        public override Task Load(Action loadAction)
        {
            loadAction();
            return Task.CompletedTask;
        }

        public static ReadOnlyFile OpenExisting(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                throw new ArgumentException("File was not found");
            var document = new ReadOnlyFile(filePath, System.IO.Path.GetFileName(filePath));
            return document;
        }
    }
}