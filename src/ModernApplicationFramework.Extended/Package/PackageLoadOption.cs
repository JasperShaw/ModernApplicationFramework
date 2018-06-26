namespace ModernApplicationFramework.Extended.Package
{
    /// <summary>
    /// The package load option specify the moment when packages are initialized 
    /// </summary>
    public enum PackageLoadOption
    {
        /// <summary>
        /// No automatic initialization
        /// </summary>
        Custom,
        /// <summary>
        /// Initializes when the application boots
        /// </summary>
        OnApplicationStart,
        /// <summary>
        /// Initializes when the UIContext of the Application was changed
        /// </summary>
        OnContextActivated
    }
}