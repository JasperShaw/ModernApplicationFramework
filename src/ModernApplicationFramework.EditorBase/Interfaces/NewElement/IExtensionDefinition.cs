using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.EditorBase.Interfaces.NewElement
{
    public interface IExtensionDefinition
    {
        string ApplicationContext { get; }

        string Description { get; }

        BitmapSource MediumThumbnailImage { get; }

        BitmapSource SmallThumbnailImage { get; }

        string Name { get; }

        string TemplateName { get; }

        int SortOrder { get; }
    }
}