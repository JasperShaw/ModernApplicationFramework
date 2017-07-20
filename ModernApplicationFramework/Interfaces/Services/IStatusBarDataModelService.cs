namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// This interfaces combines the <see cref="IStatusBarService"/> and the <see cref="IStatusBarDataModel"/> interface
    /// </summary>
    public interface IStatusBarDataModelService : IStatusBarService, IStatusBarDataModel
    {
        
    }
}