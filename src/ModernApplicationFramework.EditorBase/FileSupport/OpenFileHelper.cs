using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.Dialogs;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public static class OpenFileHelper
    {
        public static IEnumerable<OpenFileArguments> ShowOpenFilesDialog()
        {
            var fdm = IoC.Get<IFileDefinitionManager>();

            var supportedFileDefinitons =
                fdm.SupportedFileDefinitions.Where(x => x.SupportedFileOperation.HasFlag(SupportedFileOperation.Open));

            var filterData = BuildFilter(supportedFileDefinitons);

            var dialog = new OpenWithFileDialog
            {
                IsCustom = true,
                Multiselect = true,
                Filter = filterData.Filter,
                FilterIndex = filterData.Index
            };

            dialog.ShowDialog(Application.Current.MainWindow);
            
            return null;
        }

        internal static FilterData BuildFilter(IEnumerable<ISupportedFileDefinition> fileDefinitions)
        {
            var filter = new FilterData(string.Empty, 0);
            return filter;
        }

        internal struct FilterData
        {
            public int Index { get; }

            public string Filter { get; }

            public FilterData(string filter, int selectIndex)
            {
                Filter = filter;
                Index = selectIndex;
            }
        }
    }
}
