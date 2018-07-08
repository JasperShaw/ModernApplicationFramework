using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Demo.Modules.UndoRedoTest;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace ModernApplicationFramework.Extended.Demo.Modules.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class TestCommandDefinition : CommandDefinition
    {
        public TestCommandDefinition()
        {
            Command = new TestCommand();

            DefaultKeyGestures = new[]
            {
                new MultiKeyGesture(new List<KeySequence>
                {
                    new KeySequence(ModifierKeys.Control, Key.W),
                    new KeySequence(Key.K)
                })
            };

            DefaultGestureScope = UndoRedoViewModel.UndoRedoScope;
        }

        public override string IconId => null;
        public override CommandCategory Category => new CommandCategory("Test");
        public override Guid Id => new Guid("{837D016F-1B20-45AC-B86F-BEE2555406B0}");
        public override Uri IconSource => null;
        public override string Name => "MultiHotKey";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => Name;

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }
    }

    internal class TestCommand : CommandDefinitionCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var s = new Bitmap(Properties.Resources.png_undo_16_16);

            var w = new TestWindow
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
    }
}
