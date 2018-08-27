using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Extendes an object to support internal, unlocalized names
    /// </summary>
    internal interface IHasInternalName : INotifyPropertyChanged
    {
        /// <summary>
        /// The unlocalized internal name of the object
        /// </summary>
        string InternalName { get; set; }

        bool InheritInternalName { get; }
    }
}