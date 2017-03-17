﻿using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class DialogButton: System.Windows.Controls.Button
    {
        static DialogButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogButton), new FrameworkPropertyMetadata(typeof(DialogButton)));
        }
    }
}
