using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors
{
    public class BitmapSourceEditorViewModel : EditorBase<BitmapSource>, ILabelledInspector
    {
        //TODO: Decide what to do with this...
        //public IEnumerable<IResult> Choose()
        //{
        //    var fileDialog = new OpenFileDialog();
        //    yield return Show.CommonDialog(fileDialog);

        //    using (var stream = fileDialog.OpenFile())
        //    {
        //        var result = new BitmapImage();
        //        result.BeginInit();
        //        result.CacheOption = BitmapCacheOption.OnLoad;
        //        result.StreamSource = stream;
        //        result.EndInit();
        //        result.Freeze();

        //        Value = result;
        //    }
        //}
    }
}