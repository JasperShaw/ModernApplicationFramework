﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(CommandBarItemDefinition))]
    public sealed class NewCopyCommandDefinition : CommandDefinition
    {
        public NewCopyCommandDefinition() : base(new NewCopyCommand())
        {
        }

        public override CommandBarCategory Category => new CommandBarCategory("Test");
        public override Guid Id => new Guid("{2FBAE249-6E98-463A-AEE8-44B2A106F768}");
        public override string Name => "TestCopyCommand";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => Name;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(Editor.TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.C, ModifierKeys.Control))
        });
    }

    internal class NewCopyCommand : CommandDefinitionCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            MessageBox.Show("Overwritten Ctrl+C Executed");
        }
    }
}