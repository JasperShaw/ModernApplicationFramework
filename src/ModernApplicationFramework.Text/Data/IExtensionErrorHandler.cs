using System;

namespace ModernApplicationFramework.Text.Data
{
    public interface IExtensionErrorHandler
    {
        void HandleError(object sender, Exception exception);
    }
}