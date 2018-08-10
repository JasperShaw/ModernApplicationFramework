using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public abstract class ClassificationFormatDefinition : EditorFormatDefinition
    {
        public const string BackgroundOpacityId = "BackgroundOpacity";
        public const string CultureInfoId = "CultureInfo";
        public const double DefaultBackgroundOpacity = 0.8;
        public const double DefaultHighContrastBackgroundOpacity = 0.5;
        public const string FontHintingSizeId = "FontHintingSize";
        public const string FontRenderingSizeId = "FontRenderingSize";
        public const string ForegroundOpacityId = "ForegroundOpacity";
        public const string IsBoldId = "IsBold";
        public const string IsItalicId = "IsItalic";
        public const string TextDecorationsId = "TextDecorations";
        public const string TextEffectsId = "TextEffects";
        public const string TypefaceId = "Typeface";

        public double? BackgroundOpacity { get; protected set; }

        public CultureInfo CultureInfo { get; protected set; }

        public double? FontHintingSize { get; protected set; }

        public double? FontRenderingSize { get; protected set; }

        public Typeface FontTypeface { get; protected set; }

        public double? ForegroundOpacity { get; protected set; }

        public bool? IsBold { get; protected set; }

        public bool? IsItalic { get; protected set; }

        public TextDecorationCollection TextDecorations { get; protected set; }

        public TextEffectCollection TextEffects { get; protected set; }

        protected override ResourceDictionary CreateResourceDictionaryFromDefinition()
        {
            var resourceDictionary = new ResourceDictionary();
            AddOverridableProperties(resourceDictionary);
            if (ForegroundBrush != null)
            {
                resourceDictionary["Foreground"] = ForegroundBrush;
                if (ForegroundBrush.Opacity != 1.0)
                    resourceDictionary["ForegroundOpacity"] = ForegroundBrush.Opacity;
            }

            if (BackgroundBrush != null)
            {
                resourceDictionary["Background"] = BackgroundBrush;
                if (BackgroundBrush.Opacity != 1.0)
                    resourceDictionary["BackgroundOpacity"] = BackgroundBrush.Opacity;
            }

            if (FontTypeface != null)
            {
                resourceDictionary.Add("Typeface", FontTypeface);
                if (FontTypeface.Weight == FontWeights.Bold)
                    resourceDictionary["IsBold"] = true;
                if (FontTypeface.Style == FontStyles.Italic)
                    resourceDictionary["IsItalic"] = true;
            }

            if (FontRenderingSize.HasValue)
                resourceDictionary.Add("FontRenderingSize", FontRenderingSize.Value);
            if (FontHintingSize.HasValue)
                resourceDictionary.Add("FontHintingSize", FontHintingSize.Value);
            if (TextDecorations != null)
                resourceDictionary.Add("TextDecorations", TextDecorations);
            if (TextEffects != null)
                resourceDictionary.Add("TextEffects", TextEffects);
            if (CultureInfo != null)
                resourceDictionary.Add("CultureInfo", CultureInfo);
            return resourceDictionary;
        }

        private void AddOverridableProperties(ResourceDictionary resourceDictionary)
        {
            double? nullable1;
            if (ForegroundOpacity.HasValue)
            {
                var resourceDictionary1 = resourceDictionary;
                nullable1 = ForegroundOpacity;
                // ISSUE: variable of a boxed type
                var local = (ValueType) nullable1.Value;
                resourceDictionary1.Add(ForegroundOpacityId, local);
            }

            nullable1 = BackgroundOpacity;
            if (nullable1.HasValue)
            {
                var resourceDictionary1 = resourceDictionary;
                nullable1 = BackgroundOpacity;
                var local = (ValueType) nullable1.Value;
                resourceDictionary1.Add(BackgroundOpacityId, local);
            }

            bool? nullable2;
            if (IsBold.HasValue)
            {
                var resourceDictionary1 = resourceDictionary;
                nullable2 = IsBold;
                var local = (ValueType) nullable2.Value;
                resourceDictionary1.Add(IsBoldId, local);
            }

            nullable2 = IsItalic;
            if (nullable2.HasValue)
            {
                var resourceDictionary1 = resourceDictionary;
                nullable2 = IsItalic;
                var local = (ValueType) nullable2.Value;
                resourceDictionary1.Add(IsItalicId, local);
            }

            Color? nullable3;
            if (ForegroundColor.HasValue)
            {
                var resourceDictionary1 = resourceDictionary;
                nullable3 = ForegroundColor;
                var solidColorBrush = new SolidColorBrush(nullable3.Value);
                resourceDictionary1["Foreground"] = solidColorBrush;
            }

            nullable3 = BackgroundColor;
            if (!nullable3.HasValue)
                return;
            var resourceDictionary2 = resourceDictionary;
            nullable3 = BackgroundColor;
            var solidColorBrush1 = new SolidColorBrush(nullable3.Value);
            resourceDictionary2["Background"] = solidColorBrush1;
        }
    }
}