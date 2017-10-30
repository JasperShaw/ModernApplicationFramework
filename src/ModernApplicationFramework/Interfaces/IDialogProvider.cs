namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// Provides a set of methods to display messages to the UI
    /// </summary>
	public interface IDialogProvider
	{
        /// <summary>
        /// Simple message
        /// </summary>
        /// <param name="message">The message to display</param>
		void ShowMessage(string message);

        /// <summary>
        /// Offers a Yes/No question and displays it.
        /// </summary>
        /// <param name="message">The question to ask</param>
        /// <returns>The result of the question</returns>
		bool Ask(string message);

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The error message</param>
		void ShowError(string message);

        /// <summary>
        /// Shows an information message
        /// </summary>
        /// <param name="message">The information</param>
		void Inform(string message);

        /// <summary>
        /// Shows a warning message
        /// </summary>
        /// <param name="message">The warning</param>
		void Warn(string message);
	}
}