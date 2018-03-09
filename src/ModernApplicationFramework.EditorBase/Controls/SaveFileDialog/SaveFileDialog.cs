using System;
using System.IO;
using System.Linq;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.EditorBase.Controls.SaveFileDialog
{    
    public class SaveFileDialog : NativeSaveFileDialog
    {
        protected override void OnTypeChange(IFileDialog dialog)
        {
            var filterData = FilterData.CreateFromFilter(Filter);
            dialog.GetFileTypeIndex(out var index);
            var entry = filterData.EntryAt(index);

            var ext = entry.IsAnyFilter() ? DefaultExt : entry.Extensions.FirstOrDefault();

            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.StartsWith(".", StringComparison.CurrentCulture))
                    ext = ext.Substring(1);
                else if (ext.Length == 0)
                    ext = null;
                dialog.SetDefaultExtension(ext);
            }

            // Resetting the name is required at this point.
            dialog.GetFileName(out var fileName);
            if (string.IsNullOrEmpty(fileName))
                fileName = FileName;
            var fileNameWihtoutExtension = Path.GetFileNameWithoutExtension(fileName);
            dialog.SetFileName($"{fileNameWihtoutExtension}.{ext}");
        }
    }
}
