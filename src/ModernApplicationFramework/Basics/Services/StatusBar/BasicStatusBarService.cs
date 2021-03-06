﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services.StatusBar
{
    [Export(typeof(IStatusBarDataModelService))]
    public sealed class BasicStatusBarService : AbstractStatusBarService
    {
        protected override void SetTextInternal(int index, string text)
        {
            Text = text;
        }

        protected override string GetTextInternal(int index)
        {
            return Text;
        }
    }
}