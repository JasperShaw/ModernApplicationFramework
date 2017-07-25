using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Utilities.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class TestCommandDefinition : CommandDefinition
    {
        public TestCommandDefinition()
        {
            var command = new UICommand(Test, CanTest, new MultiKeyGesture(new[] {Key.W, Key.K}, ModifierKeys.Control));
            command.Category = new CommandGestureCategory("RedoUndo");
            Command = command;
            ShortcutText = command.GestureText;
        }

        private bool CanTest()
        {
            return true;
        }

        private void Test()
        {

            var s = new Bitmap(Properties.Resources.png_undo_16_16);




            var w = new TestWindw
            {
                Background = Brushes.Green,
                Grid =
                {
                    Margin = new Thickness(50),
                    Background = Brushes.Red
                }
            };


            var b = ImageThemingUtilities.GetThemedBitmap(s, ImageThemingUtilities.GetImageBackgroundColor(w.Grid).ToRgba());

            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                b.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(b.Width, b.Height));
            w.Icon = bs;


            w.ShowDialog();


            //new FolderBrowserDialog().ShowDialog();
        }

        public BitmapSource Convert(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
        public override string IconId => null;
        public override CommandCategory Category => null;
        public override Uri IconSource => null;
        public override string Name => "MultiHotKey";
        public override string Text => Name;
        public override string ToolTip => Name;

        public override ICommand Command { get; }
    }
}
