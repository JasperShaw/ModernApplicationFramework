using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;
using ModernApplicationFramework.EditorBase.Interfaces.Services;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileSystemMruManager : MruManager<FileSystemMruItem>
    {
        public FileSystemMruManager(int maxCount) : base(maxCount)
        {
        }

        protected override FileSystemMruItem CreateItem(string persistenceData)
        {
            return FileSystemMruItem.Create(persistenceData);
        }

        protected sealed override bool IsValidMruItem(string stringData)
        {
            try
            {
                return FileSystemMruItem.TryParsePath(stringData, out _, out _);
            }
            catch
            {
                return true;
            }
        }

        protected override void OnItemOpened(FileSystemMruItem item)
        {
            var service = IoC.Get<IOpenFileService>();
            if (service != null && !service.TryOpenFile(item.Path, item.EditorGuid))
            {
                PromptForItemRemoval(item);
            }
            base.OnItemOpened(item);
        }

        protected void PromptForItemRemoval(FileSystemMruItem item)
        {
            var name = IoC.Get<IEnvironmentVariables>().ApplicationName;
            var result = MessageBox.Show(string.Format(FileSupportResources.MessageMruFileCouldNotOpen, item.Path),
                name, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result == MessageBoxResult.No)
                return;
            var index = Items.IndexOf(item);
            if (index == -1)
                return;
            RemoveItemAt(index);
        }
    }
}