using System;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewHost
    {
        event EventHandler Closed;

        bool IsClosed { get; }

        Control HostControl { get; }

        ITextView TextView { get; }

        void Close();

        ITextViewMargin GetTextViewMargin(string marginName);

    }
}