using System.Windows.Media;
using ModernApplicationFramework.Basics.Services;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// Provides access to the environment's status bar.
    /// </summary>
    public interface IStatusBarService
    {

        /// <summary>
        /// Inhibits updates to the status text area.
        /// </summary>
        /// <param name="fFreeze">1 tells the environment to place a freeze on the status bar. 
        /// No further updates can be made until the freeze is released. 0 releases the freeze.</param>
        void FreezeOutput(int fFreeze);

        /// <summary>
        /// Returns the number of freeze holds on the status bar.
        /// </summary>
        /// <returns>Count of the number of freeze holds currently in place on the status bar.</returns>
        int GetFreezeCount();

        /// <summary>
        /// Returns the freeze state of the status bar.
        /// </summary>
        /// <returns>Flag indicating whether the status bar is frozen to new content.</returns>
        bool IsFrozen();

        /// <summary>
        /// Clears the status text area in the status bar.
        /// </summary>
        void Clear();

        /// <summary>
        /// Shows the progress of operations that take a determinate amount of time.
        /// </summary>
        /// <param name="inProgress"><see langword="true"/> while the progress bar is in use; <see langword="false"/> when complete.</param>
        /// <param name="label">Text to display in status field while the progress bar is in use.</param>
        /// <param name="complete">Number of units currently complete in the progress bar.</param>
        /// <param name="total">Total number of units for the progress bar operation.</param>
        void Progress(bool inProgress, string label, uint complete, uint total);

        /// <summary>
        /// Sets the status bar text in the main text area.
        /// </summary>
        /// <param name="text">The text to display in the status text area.</param>
        void SetText(string text);

        /// <summary>
        /// Sets the status bar text in a text area.
        /// </summary>
        /// <param name="index">The section where the text should be displayed</param>
        /// <param name="text">The text to display in the status text area.</param>
        void SetText(int index, string text);

        /// <summary>
        /// Retrieves the current status bar main text.
        /// </summary>
        /// <returns>Current status bar main text.</returns>
        string GetText();

        /// <summary>
        /// Retrieves the current status bar text.
        /// </summary>
        /// <param name="index">The section where the text should be retrieves from</param>
        /// <returns></returns>
        string GetText(int index);

        /// <summary>
        /// Sets a localized ready text in the main text area
        /// </summary>
        void SetReadyText();

        /// <summary>
        /// Sets the the visibility of the status bar.
        /// </summary>
        /// <param name="dwVisibility"><see langword="1"/> The status bar will be visible; <see langword="0"/> when it will be invisible.</param>
        int SetVisibility(uint dwVisibility);

        /// <summary>
        /// Retrieves the current visiblity state of the status bar.
        /// </summary>
        /// <returns><see langword="true"/> when the status bar is visible; <see langword="false"/> when not</returns>
        bool GetVisibility();

        /// <summary>
        /// Sets the background color of the status bar
        /// </summary>
        /// <param name="color">The color the status bar should take</param>
        void SetBackgroundColor(Color color);

        /// <summary>
        /// Sets the background color of the status bar
        /// </summary>
        /// <param name="color">A preset color the status bar should take</param>
        void SetBackgroundColor(AbstractStatusBarService.DefaultColors color);
    }
}