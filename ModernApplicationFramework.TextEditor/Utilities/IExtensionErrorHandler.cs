using System;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal interface IExtensionErrorHandler
    {
        void HandleError(object sender, Exception exception);
    }
}