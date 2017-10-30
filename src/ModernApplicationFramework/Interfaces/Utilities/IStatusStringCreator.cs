namespace ModernApplicationFramework.Interfaces.Utilities
{
    /// <summary>
    /// Factory to create localized messages with one variable object
    /// </summary>
    public interface IStatusStringCreator
    {
        /// <summary>
        /// The template text for the status text
        /// </summary>
        string StatusTextTemplate { get; set; }

        /// <summary>
        /// A special suffix pattern for plural in text
        /// </summary>
        string PluralSuffix { get; set; }

        /// <summary>
        /// Creates the message
        /// </summary>
        /// <param name="value">The variable object the message should be filled with</param>
        /// <returns>Returns the message</returns>
        string CreateMessage(object value);

        /// <summary>
        /// Creates a default message
        /// </summary>
        /// <returns>Returns the message</returns>
        string CreateDefaultMessage();
    }
}