﻿using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;

namespace ModernApplicationFramework.MVVM.Demo
{
    [Export(typeof(CommandDefinition))]
    public sealed class TestCommandDefinition : CommandDefinition
    {

        public static string GestureText = "Test";

        public TestCommandDefinition()
        {
            Command = new GestureCommand(Test, new KeyGesture(Key.Z, ModifierKeys.Control));
        }
        public override string Name => "Test";
        public override string Text => Name;
        public override string ToolTip => Name;
        public override Uri IconSource { get; }
        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        private void Test()
        {
            MessageBox.Show("Test");
        }
    }


}