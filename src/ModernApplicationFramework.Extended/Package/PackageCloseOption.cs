namespace ModernApplicationFramework.Extended.Package
{
    /// <summary>
    /// The package close option specify the moment when packages are disposed from the application 
    /// </summary>
    public enum PackageCloseOption
    {
        /// <summary>
        /// No automatic closing
        /// </summary>
        Custom,
        /// <summary>
        /// Closes when the Main Window was disposed
        /// </summary>
        OnMainWindowClosed,
        /// <summary>
        /// Closes when the Application in about the terminate
        /// </summary>
        PreviewApplicationClosed
    }
}