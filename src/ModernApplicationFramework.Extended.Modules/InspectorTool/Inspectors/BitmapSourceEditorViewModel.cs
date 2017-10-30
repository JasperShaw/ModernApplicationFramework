using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Microsoft.Win32;
using ModernApplicationFramework.Extended.ResultDialogs;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors
{
    public class BitmapSourceEditorViewModel : EditorBase<BitmapSource>, ILabelledInspector
    {
        public IEnumerable<IResult> Choose()
        {
            var fileDialog = new OpenFileDialog();
            yield return Show.CommonDialog(fileDialog);

            using (var stream = fileDialog.OpenFile())
            {
                var result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

                Value = result;
            }
        }
    }
}