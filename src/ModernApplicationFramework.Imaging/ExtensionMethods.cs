namespace ModernApplicationFramework.Imaging
{
    internal static class ExtensionMethods
    {
        internal static ImageMoniker ToInternalType(this Interop.ImageMoniker moniker)
        {
            return new ImageMoniker(moniker.CatalogGuid, moniker.Id);
        }
    }
}
