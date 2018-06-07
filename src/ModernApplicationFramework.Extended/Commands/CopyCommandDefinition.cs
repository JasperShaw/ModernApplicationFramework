using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Core.CommandFocus;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class CopyCommandDefinition : CommandDefinition
    {
        public override string NameUnlocalized => "Copy";
        public override string Text => "Copy";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{E98D986F-AACB-4BC7-A60B-E758CA847BA9}");
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override ICommand Command { get; }

        public CopyCommandDefinition()
        {
            var command = new UICommand(Copy, CanCopy);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(Key.C, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        private void Copy()
        {
            MessageBox.Show(Keyboard.FocusedElement.ToString());
        }

        private bool CanCopy()
        {
            return true;
        }
    }

    public class CopyPasteCutManager : ICopyPasteCutManager
    {
        private static CopyPasteCutManager _instance;

        public static CopyPasteCutManager Instance
        {
            get => _instance ?? (_instance = new CopyPasteCutManager());
            set => _instance = value;
        }

        public void CopyToClipboard()
        {
            
        }

        public object Copy()
        {
            return null;
        }

        public void Paste(object data)
        {
        }

        public object Cut()
        {
            return null;
        }

        public void CutToClipboard()
        {
        }
    }

    public interface ICopyPasteCutManager
    {
        void CopyToClipboard();

        object Copy();

        void Paste(object data);

        object Cut();

        void CutToClipboard();
    }


    public interface ICopyVisitor
    {

    }

    public interface ICopyVisitor<T> : ICopyVisitor where T : ICopyable
    {
        void Visit(T t);
    }

    public interface ICopyable
    {
        object Copy(ICopyVisitor visitor);
    }

    public class MyControll : ICopyable
    {
        public object Copy(ICopyVisitor visitor)
        {
            return 1;
        }
    }
}
