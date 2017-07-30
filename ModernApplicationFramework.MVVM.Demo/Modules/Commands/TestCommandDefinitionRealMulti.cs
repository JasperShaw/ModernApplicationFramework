using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest;
using ModernApplicationFramework.Utilities.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class TestCommandDefinitionRealMulti : CommandDefinition
    {
        public TestCommandDefinitionRealMulti()
        {
            var command = new UICommand(Test, CanTest);
            Command = command;

            var l = new List<KeySequence>
            {
                new KeySequence(ModifierKeys.Control, Key.A),
                new KeySequence(ModifierKeys.Control, Key.A),
            };

            var gesture = new MultiKeyGesture(l);
                     
            DefaultKeyGesture = gesture;
            DefaultGestureCategory = UndoRedoViewModel.UndoRedoCategory;
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

            w.Title = "Real Multi";

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
        public override CommandCategory Category => new CommandCategory("Test");
        public override Uri IconSource => null;
        public override string Name => "MultiHotKey";
        public override string Text => Name;
        public override string ToolTip => Name;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }
    }
}
