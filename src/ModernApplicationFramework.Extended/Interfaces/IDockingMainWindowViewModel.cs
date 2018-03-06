using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <inheritdoc cref="IMainWindowViewModel" />
    /// <summary>
    /// An <see cref="IMainWindowViewModel"/> that is host to a <see cref="IDockingHostViewModel"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IMainWindowViewModel" />
    /// <seealso cref="T:ModernApplicationFramework.Extended.Interfaces.IUseDockingHost" />
    public interface IDockingMainWindowViewModel : IMainWindowViewModel, IUseDockingHost
    {
    }
}