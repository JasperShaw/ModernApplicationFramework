﻿using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IKeyGestureHandler
    {
        void BindKeyGesture(UIElement uiElement);

        void RestoreBindings();

        KeyGesture GetPrimaryKeyGesture(GestureCommandWrapper command);
    }
}