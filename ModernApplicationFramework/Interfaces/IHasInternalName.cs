using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// Extendes an object to support internal, unlocalized names
    /// </summary>
    public interface IHasInternalName : INotifyPropertyChanged
    {
        /// <summary>
        /// The unlocalized internal name of the object
        /// </summary>
        string InternalName { get; set; }
    }
}