using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Ui.Classification;

namespace ModernApplicationFramework.Extended.Demo.Modules.EditorTheme
{
    [Guid("F7CE1D0A-09E7-4AAE-9425-B94D4B3350C3")]
    [PackageAutoLoad(UiContextGuids.ShellInitializedContextGuid)]
    [Export(typeof(IMafPackage))]
    internal class ThemeWatcher : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("F7CE1D0A-09E7-4AAE-9425-B94D4B3350C3");

        [ImportingConstructor]
        public ThemeWatcher(IThemeManager themeManager)
        {
            themeManager.OnThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object sender, Core.Events.ThemeChangedEventArgs e)
        {
            //var service = IoC.Get<IEditorFormatMapService>();
            //var map = service.GetEditorFormatMap("output");


            //var p = map.GetProperties("TextView Background");


            //var c = Colors.Green;
            //p["Background"] = new SolidColorBrush(c);
            //p["BackgroundColor"] = c;
            //map.SetProperties("TextView Background", p);


            //var cfms = IoC.Get<IClassificationFormatMapService>();
            //var cfm = cfms.GetClassificationFormatMap("output");
            //var tp = cfm.DefaultTextProperties.SetForeground(Colors.Red);
            //tp = tp.SetBackground(Colors.Blue);
            //cfm.DefaultTextProperties = tp;
        }
    }
}
