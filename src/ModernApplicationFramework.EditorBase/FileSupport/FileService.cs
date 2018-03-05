using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Export(typeof(IFileService))]
    public class FileService : IFileService
    {
        private static IFileService _instance;

        public static IFileService Instance => _instance ?? (_instance = IoC.Get<IFileService>());

        private FileService()
        {

        }
    }

    public interface IFileService
    {
    }
}
