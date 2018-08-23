using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Denotes an object to have localized, displayed text
    /// </summary>
    public interface IHasTextProperty : INotifyPropertyChanged
    {
        /// <summary>
        /// The localized object text
        /// </summary>
        string Text { get; set; }
    }
}