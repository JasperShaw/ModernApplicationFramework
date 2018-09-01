using System.Windows.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutSynchronizedContentControl : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            LayoutSynchronizer.Update(this);
        }
    }
}