using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.Dialogs;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;
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
                FilterIndex = filterData.MaxIndex
            };
            dialog.ShowDialog();
            
            return null;
        }

        internal static FilterData BuildFilter(IEnumerable<ISupportedFileDefinition> fileDefinitions)
        {
            var filter = new FilterData();

            var availableContexts = IoC.Get<IFileDefinitionContextManager>().GetRegisteredFileDefinitionContexts;

            foreach (var context in availableContexts)
            {
                var t = fileDefinitions.Where(x => x.FileContexts.Contains(context)).Select(x => x.FileExtension).ToList();
                filter.AddFilter(new FilterDataEntry(context.Context, t));
            }
            filter.AddFilterAnyFileAtEnd = true;
            filter.AddFilterAnyFile(FileSupportResources.OpenSaveFileFilterAnyText);
            return filter;
        }
    }
}
