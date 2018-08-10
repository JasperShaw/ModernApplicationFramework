using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    [Serializable]
    public sealed class TextFormattingRunProperties : TextRunProperties, ISerializable, IObjectReference
    {
        [NonSerialized]
        private static readonly List<TextFormattingRunProperties> ExistingProperties = new List<TextFormattingRunProperties>();
        [NonSerialized]
        private static readonly TextFormattingRunProperties EmptyProperties = new TextFormattingRunProperties();
        [NonSerialized]
        private static readonly TextEffectCollection EmptyTextEffectCollection = new TextEffectCollection();
        [NonSerialized]
        private static readonly TextDecorationCollection EmptyTextDecorationCollection = new TextDecorationCollection();
        [NonSerialized]
        private Typeface _typeface;
        [NonSerialized]
        private double? _size;
        [NonSerialized]
        private double? _hintingSize;
        [NonSerialized]
        private double? _foregroundOpacity;
        [NonSerialized]
        private double? _backgroundOpacity;
        [NonSerialized]
        private Brush _foregroundBrush;
        [NonSerialized]
        private Brush _backgroundBrush;
        [NonSerialized]
        private TextDecorationCollection _textDecorations;
        [NonSerialized]
        private TextEffectCollection _textEffects;
        [NonSerialized]
        private CultureInfo _cultureInfo;
        [NonSerialized]
        private bool? _bold;
        [NonSerialized]
        private bool? _italic;

        static TextFormattingRunProperties()
        {
            EmptyTextEffectCollection.Freeze();
            EmptyTextDecorationCollection.Freeze();
        }

        internal TextFormattingRunProperties()
        {
        }

        internal TextFormattingRunProperties(SerializationInfo info, StreamingContext context)
        {
            _foregroundBrush = (Brush)GetObjectFromSerializationInfo(nameof(ForegroundBrush), info);
            _backgroundBrush = (Brush)GetObjectFromSerializationInfo(nameof(BackgroundBrush), info);
            _size = (double?)GetObjectFromSerializationInfo("FontRenderingSize", info);
            _hintingSize = (double?)GetObjectFromSerializationInfo("FontHintingSize", info);
            _foregroundOpacity = (double?)GetObjectFromSerializationInfo(nameof(ForegroundOpacity), info);
            _backgroundOpacity = (double?)GetObjectFromSerializationInfo(nameof(BackgroundOpacity), info);
            _italic = (bool?)GetObjectFromSerializationInfo(nameof(Italic), info);
            _bold = (bool?)GetObjectFromSerializationInfo(nameof(Bold), info);
            _textDecorations = (TextDecorationCollection)GetObjectFromSerializationInfo(nameof(TextDecorations), info);
            _textEffects = (TextEffectCollection)GetObjectFromSerializationInfo(nameof(TextEffects), info);
            var serializationInfo1 = (string)GetObjectFromSerializationInfo("CultureInfoName", info);
            _cultureInfo = serializationInfo1 == null ? null : new CultureInfo(serializationInfo1);
            if (GetObjectFromSerializationInfo("FontFamily", info) is FontFamily serializationInfo2)
            {
                var serializationInfo3 = (FontStyle)GetObjectFromSerializationInfo("Typeface.Style", info);
                var serializationInfo4 = (FontWeight)GetObjectFromSerializationInfo("Typeface.Weight", info);
                var serializationInfo5 = (FontStretch)GetObjectFromSerializationInfo("Typeface.Stretch", info);
                _typeface = new Typeface(serializationInfo2, serializationInfo3, serializationInfo4, serializationInfo5);
            }
            if (_size.HasValue && _size.Value <= 0.0)
                _size = 16.0;
            if (!_hintingSize.HasValue || _hintingSize.Value > 0.0)
                return;
            _hintingSize = 16.0;
        }

        internal TextFormattingRunProperties(Brush foreground, Brush background, Typeface typeface, double? size, double? hintingSize, TextDecorationCollection textDecorations, TextEffectCollection textEffects, CultureInfo cultureInfo)
        {
            if (size.HasValue && size.Value <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(size), "size should be positive or null");
            if (hintingSize.HasValue && hintingSize.Value <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(hintingSize), "hintingSize should be positive or null");
            _foregroundBrush = foreground;
            _backgroundBrush = background;
            _typeface = typeface;
            _size = size;
            _hintingSize = hintingSize;
            _textDecorations = textDecorations;
            _textEffects = textEffects;
            _cultureInfo = cultureInfo;
        }

        internal TextFormattingRunProperties(TextFormattingRunProperties toCopy)
        {
            _foregroundBrush = toCopy._foregroundBrush;
            _backgroundBrush = toCopy._backgroundBrush;
            _typeface = toCopy._typeface;
            _size = toCopy._size;
            _hintingSize = toCopy._hintingSize;
            _textDecorations = toCopy._textDecorations;
            _textEffects = toCopy._textEffects;
            _cultureInfo = toCopy._cultureInfo;
            _backgroundOpacity = toCopy._backgroundOpacity;
            _foregroundOpacity = toCopy._foregroundOpacity;
            _italic = toCopy._italic;
            _bold = toCopy._bold;
            if (_size.HasValue && _size.Value <= 0.0)
                _size = 16.0;
            if (!_hintingSize.HasValue || _hintingSize.Value > 0.0)
                return;
            _hintingSize = 16.0;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static TextFormattingRunProperties CreateTextFormattingRunProperties()
        {
            return FindOrCreateProperties(EmptyProperties);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static TextFormattingRunProperties CreateTextFormattingRunProperties(Typeface typeface, double size, Color foreground)
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(new SolidColorBrush(foreground), null, typeface, size, new double?(), null, null, null));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TextFormattingRunProperties CreateTextFormattingRunProperties(Brush foreground, Brush background, Typeface typeface, double? size, double? hintingSize, TextDecorationCollection textDecorations, TextEffectCollection textEffects, CultureInfo cultureInfo)
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(foreground, background, typeface, size, hintingSize, textDecorations, textEffects, cultureInfo));
        }

        public override Brush BackgroundBrush => _backgroundBrush ?? Brushes.Transparent;

        public override CultureInfo CultureInfo => _cultureInfo ?? CultureInfo.CurrentCulture;

        public override double FontHintingEmSize => _hintingSize ?? 16.0;

        public override double FontRenderingEmSize => _size ?? 16.0;

        public override Brush ForegroundBrush => _foregroundBrush ?? Brushes.Transparent;

        public bool Italic
        {
            get
            {
                if (!_italic.HasValue)
                    return false;
                return _italic.Value;
            }
        }

        public bool Bold
        {
            get
            {
                if (!_bold.HasValue)
                    return false;
                return _bold.Value;
            }
        }

        public double ForegroundOpacity
        {
            get
            {
                if (!_foregroundOpacity.HasValue)
                    return 1.0;
                return _foregroundOpacity.Value;
            }
        }

        public double BackgroundOpacity
        {
            get
            {
                if (!_backgroundOpacity.HasValue)
                    return 1.0;
                return _backgroundOpacity.Value;
            }
        }

        public override TextDecorationCollection TextDecorations => _textDecorations ?? EmptyTextDecorationCollection;

        public override TextEffectCollection TextEffects => _textEffects ?? EmptyTextEffectCollection;

        public override Typeface Typeface => _typeface;

        public bool BackgroundBrushEmpty => _backgroundBrush == null;

        public bool BackgroundOpacityEmpty => !_backgroundOpacity.HasValue;

        public bool ForegroundOpacityEmpty => !_foregroundOpacity.HasValue;

        public bool BoldEmpty => !_bold.HasValue;

        public bool ItalicEmpty => !_italic.HasValue;

        public bool CultureInfoEmpty => _cultureInfo == null;

        public bool FontHintingEmSizeEmpty => !_hintingSize.HasValue;

        public bool FontRenderingEmSizeEmpty => !_size.HasValue;

        public bool ForegroundBrushEmpty => _foregroundBrush == null;

        public bool TextDecorationsEmpty => _textDecorations == null;

        public bool TextEffectsEmpty => _textEffects == null;

        public bool TypefaceEmpty => _typeface == null;

        public TextFormattingRunProperties ClearBold()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _bold = new bool?()
            });
        }

        public TextFormattingRunProperties ClearItalic()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _italic = new bool?()
            });
        }

        public TextFormattingRunProperties ClearForegroundOpacity()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _foregroundOpacity = new double?()
            });
        }

        public TextFormattingRunProperties ClearBackgroundOpacity()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _backgroundOpacity = new double?()
            });
        }

        public TextFormattingRunProperties ClearBackgroundBrush()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _backgroundBrush = null
            });
        }

        public TextFormattingRunProperties ClearCultureInfo()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _cultureInfo = null
            });
        }

        public TextFormattingRunProperties ClearFontHintingEmSize()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _hintingSize = new double?()
            });
        }

        public TextFormattingRunProperties ClearFontRenderingEmSize()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _size = new double?()
            });
        }

        public TextFormattingRunProperties ClearForegroundBrush()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _foregroundBrush = null
            });
        }

        public TextFormattingRunProperties ClearTextDecorations()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _textDecorations = null
            });
        }

        public TextFormattingRunProperties ClearTextEffects()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _textEffects = null
            });
        }

        public TextFormattingRunProperties ClearTypeface()
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _typeface = null
            });
        }

        public TextFormattingRunProperties SetBackgroundBrush(Brush brush)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            var properties = new TextFormattingRunProperties(this) {_backgroundBrush = brush};
            properties.FreezeBackgroundBrush();
            return FindOrCreateProperties(properties);
        }

        public TextFormattingRunProperties SetBackground(Color background)
        {
            return SetBackgroundBrush(new SolidColorBrush(background));
        }

        public TextFormattingRunProperties SetCultureInfo(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                throw new ArgumentNullException(nameof(cultureInfo));
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _cultureInfo = cultureInfo
            });
        }

        public TextFormattingRunProperties SetFontHintingEmSize(double hintingSize)
        {
            if (hintingSize <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(hintingSize));
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _hintingSize = hintingSize
            });
        }

        public TextFormattingRunProperties SetFontRenderingEmSize(double renderingSize)
        {
            if (renderingSize <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(renderingSize));
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _size = renderingSize
            });
        }

        public TextFormattingRunProperties SetForegroundBrush(Brush brush)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            var properties = new TextFormattingRunProperties(this) {_foregroundBrush = brush};
            properties.FreezeForegroundBrush();
            return FindOrCreateProperties(properties);
        }

        public TextFormattingRunProperties SetForeground(Color foreground)
        {
            return SetForegroundBrush(new SolidColorBrush(foreground));
        }

        public TextFormattingRunProperties SetTextDecorations(TextDecorationCollection textDecorations)
        {
            if (textDecorations == null)
                throw new ArgumentNullException(nameof(textDecorations));
            var properties = new TextFormattingRunProperties(this) {_textDecorations = textDecorations};
            properties.FreezeTextDecorations();
            return FindOrCreateProperties(properties);
        }

        public TextFormattingRunProperties SetTextEffects(TextEffectCollection textEffects)
        {
            if (textEffects == null)
                throw new ArgumentNullException(nameof(textEffects));
            var properties = new TextFormattingRunProperties(this) {_textEffects = textEffects};
            properties.FreezeTextEffects();
            return FindOrCreateProperties(properties);
        }

        public TextFormattingRunProperties SetTypeface(Typeface typeface)
        {
            if (typeface == null)
                throw new ArgumentNullException(nameof(typeface));
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _typeface = typeface
            });
        }

        public TextFormattingRunProperties SetForegroundOpacity(double opacity)
        {
            if (opacity < 0.0 || opacity > 1.0)
                throw new ArgumentOutOfRangeException(nameof(opacity));
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _foregroundOpacity = opacity
            });
        }

        public TextFormattingRunProperties SetBackgroundOpacity(double opacity)
        {
            if (opacity < 0.0 || opacity > 1.0)
                throw new ArgumentOutOfRangeException(nameof(opacity));
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _backgroundOpacity = opacity
            });
        }

        public TextFormattingRunProperties SetBold(bool isBold)
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _bold = isBold
            });
        }

        public TextFormattingRunProperties SetItalic(bool isItalic)
        {
            return FindOrCreateProperties(new TextFormattingRunProperties(this)
            {
                _italic = isItalic
            });
        }

        public bool ForegroundBrushSame(Brush brush)
        {
            return BrushesEqual(_foregroundBrush, brush);
        }

        public bool BackgroundBrushSame(Brush brush)
        {
            return BrushesEqual(_backgroundBrush, brush);
        }

        public bool SameSize(TextFormattingRunProperties other)
        {
            if (other == null)
                return false;
            var size1 = _size;
            var size2 = other._size;
            if (size1.GetValueOrDefault() != size2.GetValueOrDefault())
                return false;
            return size1.HasValue == size2.HasValue;
        }

        internal static bool BrushesEqual(Brush brush, Brush other)
        {
            if (brush == other)
                return true;
            if (brush == null || other == null)
                return false;
            if (brush.Opacity == 0.0 && other.Opacity == 0.0)
                return true;
            var solidColorBrush1 = brush as SolidColorBrush;
            var solidColorBrush2 = other as SolidColorBrush;
            if (solidColorBrush1 == null || solidColorBrush2 == null)
                return brush.Equals(other);
            if (solidColorBrush1.Color.A == 0 && solidColorBrush2.Color.A == 0)
                return true;
            if (solidColorBrush1.Color == solidColorBrush2.Color)
                return Math.Abs(solidColorBrush1.Opacity - solidColorBrush2.Opacity) < 0.01;
            return false;
        }

        private static bool TypefacesEqual(Typeface typeface, Typeface other)
        {
            if (typeface == null)
                return other == null;
            return typeface.Equals(other);
        }

        internal static TextFormattingRunProperties FindOrCreateProperties(TextFormattingRunProperties properties)
        {
            var formattingRunProperties = ExistingProperties.Find(properties.IsEqual);
            if (formattingRunProperties != null)
                return formattingRunProperties;
            properties.FreezeEverything();
            ExistingProperties.Add(properties);
            return properties;
        }

        private bool IsEqual(TextFormattingRunProperties other)
        {
            var size = _size;
            var nullable1 = other._size;
            if ((size.GetValueOrDefault() == nullable1.GetValueOrDefault() ? (size.HasValue == nullable1.HasValue ? 1 : 0) : 0) != 0)
            {
                nullable1 = _hintingSize;
                var nullable2 = other._hintingSize;
                if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue == nullable2.HasValue ? 1 : 0) : 0) != 0 && TypefacesEqual(_typeface, other._typeface) && (_cultureInfo == other._cultureInfo && _textDecorations == other._textDecorations) && _textEffects == other._textEffects)
                {
                    var italic = _italic;
                    var nullable3 = other._italic;
                    if ((italic.GetValueOrDefault() == nullable3.GetValueOrDefault() ? (italic.HasValue == nullable3.HasValue ? 1 : 0) : 0) != 0)
                    {
                        nullable3 = _bold;
                        var bold = other._bold;
                        if ((nullable3.GetValueOrDefault() == bold.GetValueOrDefault() ? (nullable3.HasValue == bold.HasValue ? 1 : 0) : 0) != 0)
                        {
                            nullable2 = _foregroundOpacity;
                            nullable1 = other._foregroundOpacity;
                            if ((nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? (nullable2.HasValue == nullable1.HasValue ? 1 : 0) : 0) != 0)
                            {
                                nullable1 = _backgroundOpacity;
                                nullable2 = other._backgroundOpacity;
                                if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue == nullable2.HasValue ? 1 : 0) : 0) != 0 && BrushesEqual(_foregroundBrush, other._foregroundBrush))
                                    return BrushesEqual(_backgroundBrush, other._backgroundBrush);
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void FreezeBackgroundBrush()
        {
            if (_backgroundBrush == null || !_backgroundBrush.CanFreeze)
                return;
            _backgroundBrush.Freeze();
        }

        private void FreezeEverything()
        {
            FreezeForegroundBrush();
            FreezeBackgroundBrush();
            FreezeTextEffects();
            FreezeTextDecorations();
        }

        private void FreezeForegroundBrush()
        {
            if (_foregroundBrush == null || !_foregroundBrush.CanFreeze)
                return;
            _foregroundBrush.Freeze();
        }

        private void FreezeTextDecorations()
        {
            if (_textDecorations == null || !_textDecorations.CanFreeze)
                return;
            _textDecorations.Freeze();
        }

        private void FreezeTextEffects()
        {
            if (_textEffects == null || !_textEffects.CanFreeze)
                return;
            _textEffects.Freeze();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            info.AddValue("BackgroundBrush", BackgroundBrushEmpty ? "null" : XamlWriter.Save(BackgroundBrush));
            info.AddValue("ForegroundBrush", ForegroundBrushEmpty ? "null" : XamlWriter.Save(ForegroundBrush));
            info.AddValue("FontHintingSize", FontHintingEmSizeEmpty ? "null" : XamlWriter.Save(FontHintingEmSize));
            info.AddValue("FontRenderingSize", FontRenderingEmSizeEmpty ? "null" : XamlWriter.Save(FontRenderingEmSize));
            info.AddValue("TextDecorations", TextDecorationsEmpty ? "null" : XamlWriter.Save(TextDecorations));
            info.AddValue("TextEffects", TextEffectsEmpty ? "null" : XamlWriter.Save(TextEffects));
            info.AddValue("CultureInfoName", CultureInfoEmpty ? "null" : XamlWriter.Save(CultureInfo.Name));
            info.AddValue("FontFamily", TypefaceEmpty ? "null" : XamlWriter.Save(Typeface.FontFamily));
            info.AddValue("Italic", ItalicEmpty ? "null" : XamlWriter.Save(Italic));
            info.AddValue("Bold", BoldEmpty ? "null" : XamlWriter.Save(Bold));
            info.AddValue("ForegroundOpacity", ForegroundOpacityEmpty ? "null" : XamlWriter.Save(ForegroundOpacity));
            info.AddValue("BackgroundOpacity", BackgroundOpacityEmpty ? "null" : XamlWriter.Save(BackgroundOpacity));
            if (TypefaceEmpty)
                return;
            info.AddValue("Typeface.Style", XamlWriter.Save(Typeface.Style));
            info.AddValue("Typeface.Weight", XamlWriter.Save(Typeface.Weight));
            info.AddValue("Typeface.Stretch", XamlWriter.Save(Typeface.Stretch));
        }

        private object GetObjectFromSerializationInfo(string name, SerializationInfo info)
        {
            var xamlText = info.GetString(name);
            if (xamlText == "null")
                return null;
            return XamlReader.Parse(xamlText);
        }

        public object GetRealObject(StreamingContext context)
        {
            return FindOrCreateProperties(this);
        }
    }
}