namespace ModernApplicationFramework.EditorBase.Interfaces.Services
{
    public interface IUniqueNameCreator<in T>
    {
        /// <summary>
        /// Returns an unique name in this application instance.
        /// </summary>
        /// <param name="data">The template data.</param>
        /// <returns>The unique name</returns>
        string GetUniqueName(T data);

        /// <summary>
        /// Flushes the store.
        /// </summary>
        void Flush();
    }
}