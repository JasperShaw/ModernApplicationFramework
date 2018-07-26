using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class EditorFormatDefinition
    {
        public const string BackgroundBrushId = "Background";
        public const string ForegroundBrushId = "Foreground";
        public const string BackgroundColorId = "BackgroundColor";
        public const string ForegroundColorId = "ForegroundColor";

        public Color? ForegroundColor { get; protected set; }

        public Color? BackgroundColor { get; protected set; }

        public Brush BackgroundBrush { get; protected set; }

        public Brush ForegroundBrush { get; protected set; }

        public bool? ForegroundCustomizable { get; protected set; }

        public bool? BackgroundCustomizable { get; protected set; }

        public string DisplayName { get; protected set; }

        public ResourceDictionary CreateResourceDictionary()
        {
            return CreateResourceDictionaryFromDefinition();
        }

        protected virtual ResourceDictionary CreateResourceDictionaryFromDefinition()
        {
            var resourceDictionary = new ResourceDictionary();
            Brush brush1 = null;
            Brush brush2 = null;
            if (ForegroundColor.HasValue)
            {
                resourceDictionary["ForegroundColor"] = ForegroundColor;
                brush1 = new SolidColorBrush(ForegroundColor.Value);
            }
            var backgroundColor = BackgroundColor;
            if (backgroundColor.HasValue)
            {
                resourceDictionary["BackgroundColor"] = BackgroundColor;
                backgroundColor = BackgroundColor;
                brush2 = new SolidColorBrush(backgroundColor.Value);
            }
            if (ForegroundBrush != null)
                brush1 = ForegroundBrush.Clone();
            if (BackgroundBrush != null)
                brush2 = BackgroundBrush.Clone();
            if (brush1 != null)
            {
                brush1.Freeze();
                resourceDictionary["Foreground"] = brush1;
            }
            if (brush2 != null)
            {
                brush2.Freeze();
                resourceDictionary["Background"] = brush2;
            }
            return resourceDictionary;
        }
    }
}