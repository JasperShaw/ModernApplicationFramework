using System.Threading.Tasks;
using System.Xml;

namespace ModernApplicationFramework.Interfaces.Settings
{
    /// <summary>
    /// An <see cref="IPropteryValueManager"/> allows to get or set values of an 'PropertyValue' <see cref="XmlElement"/> in an <see cref="XmlDocument"/>. The 
    /// </summary>
    public interface IPropteryValueManager
    {
        /// <summary>
        /// Gets the value of a PropertyValue element. Creates a new ProertyValue if it could not be found
        /// </summary>
        /// <typeparam name="T">The type of the value expected to get.</typeparam>
        /// <param name="propertyPath">The path pointing to the PropertyValue inside the document</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">Gets filled with the value of the property. Will be the default value of the given type when the property was created</param>
        /// <param name="defaultValue">The default value to set when the PropertyValue was created</param>
        /// <param name="navigateAttributeWise"><see langword="true"/> when the <see cref="propertyPath"/> contains the attribute names; <see langword="false"/> when the <see cref="propertyPath"/> contains the element names</param>
        /// <returns></returns>
        GetValueResult GetOrCreatePropertyValue<T>(string propertyPath, string propertyName, out T value, T defaultValue,
            bool navigateAttributeWise);

        /// <summary>
        /// Gets the value of a PropertyValue element. Output value will be default value of type T when element was not found.
        /// </summary>
        /// <typeparam name="T">The type of the value expected to get.</typeparam>
        /// <param name="propertyPath">The path pointing to the PropertyValue inside the document</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">Gets filled with the value of the property. Will be the default value of the given type when the property could not be found</param>
        /// <param name="navigateAttributeWise"><see langword="true"/> when the <see cref="propertyPath"/> contains the attribute names; <see langword="false"/> when the <see cref="propertyPath"/> contains the element names</param>
        /// <returns>Returns the result of this operation. Returns <see cref="GetValueResult.Missing"/> when PropertyValue could not be found</returns>
        GetValueResult GetPropertyValue<T>(string propertyPath, string propertyName, out T value,
            bool navigateAttributeWise);

        /// <summary>
        /// Async method to add a property-value setting to the settings file
        /// </summary>
        /// <param name="path">The path pointing to the PropertyValue inside the document</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">The value the PropertyValue should be applied with</param>
        /// <param name="navigateAttributeWise"><see langword="true"/> when the <see cref="path"/> contains the attribute names; <see langword="false"/> when the <see cref="path"/> contains the element names</param>
        /// <returns>Returns the task that was performed</returns>
        Task SetPropertyValueAsync(string path, string propertyName, string value, bool navigateAttributeWise);
    }
}