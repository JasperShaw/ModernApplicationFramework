using System;
using System.Windows.Controls;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewHost
    {
        event EventHandler Closed;

        bool IsClosed { get; }

        Control HostControl { get; }

        ITextView TextView { get; }

        void Close();

    }
}