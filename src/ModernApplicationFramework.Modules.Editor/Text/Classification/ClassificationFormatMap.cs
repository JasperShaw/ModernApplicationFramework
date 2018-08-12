using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Editor.Text.Classification
{
    internal sealed class ClassificationFormatMap : IClassificationFormatMap
    {
        private readonly Dictionary<string, TextFormattingRunProperties> _textFormattingPropertiesWithDefaults = new Dictionary<string, TextFormattingRunProperties>();
        private readonly Dictionary<string, string> _classificationToFormatNameMap = new Dictionary<string, string>();
        private ResourceDictionary _defaultTextProperties = new ResourceDictionary();
        private List<IClassificationType> _priorityOrder;
        private bool? _raiseEventOnBatchEnd;
        private readonly IEditorFormatMap _formatMap;
        private TextFormattingRunProperties _defaultTextFormattingRunProperties;
        private readonly IEditorOptionsFactoryService _optionsService;

        private ICollection<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> Formats { get; }

        private IClassificationTypeRegistryService ClassificationTypeRegistry { get; }

        public ClassificationFormatMap(ICollection<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> formats, IClassificationTypeRegistryService classificationTypeRegistry, IEditorFormatMap formatMap, IEditorOptionsFactoryService optionsService)
        {
            Formats = formats;
            _formatMap = formatMap;
            ClassificationTypeRegistry = classificationTypeRegistry;
            _optionsService = optionsService;
            _defaultTextProperties = _formatMap.GetProperties("Plain Text");
            EnsureDefaultTextProperties();
            _formatMap.FormatMappingChanged += OnFormatMappingChanged;
        }

        public event EventHandler<EventArgs> ClassificationFormatMappingChanged;

        public ReadOnlyCollection<IClassificationType> CurrentPriorityOrder
        {
            get
            {
                EnsurePriorityOrder();
                return _priorityOrder.AsReadOnly();
            }
        }

        public TextFormattingRunProperties DefaultTextProperties
        {
            get => _defaultTextFormattingRunProperties;
            set
            {
                if (value == null || value == _defaultTextFormattingRunProperties)
                    return;
                _defaultTextProperties = CreateRdFromTfrp(value);
                EnsureDefaultTextProperties();
                RecordStateChange();
            }
        }

        public void SwapPriorities(IClassificationType firstType, IClassificationType secondType)
        {
            var index1 = _priorityOrder.IndexOf(firstType);
            var index2 = _priorityOrder.IndexOf(secondType);
            _priorityOrder[index1] = secondType;
            _priorityOrder[index2] = firstType;
            RecordStateChange();
        }

        public TextFormattingRunProperties GetTextProperties(IClassificationType classificationType)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            if (_textFormattingPropertiesWithDefaults.TryGetValue(classificationType.Classification, out var formattingRunProperties))
                return formattingRunProperties;
            formattingRunProperties = CreateTfrpFromRd(GetResourceDictionaryFromHierarchy(classificationType), _optionsService.GlobalOptions.IsInContrastMode());
            _textFormattingPropertiesWithDefaults[classificationType.Classification] = formattingRunProperties;
            return formattingRunProperties;
        }

        public string GetEditorFormatMapKey(IClassificationType classificationType)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            var classification = classificationType.Classification;
            var str = GetFormatNameFromClassification(classification);
            if (string.IsNullOrEmpty(str))
            {
                str = classification;
                _classificationToFormatNameMap[classification] = str;
            }
            return str;
        }

        public TextFormattingRunProperties GetExplicitTextProperties(IClassificationType classificationType)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            var isInContrastMode = _optionsService.GlobalOptions.IsInContrastMode();
            return CreateTfrpFromRd(_formatMap.GetProperties(GetEditorFormatMapKey(classificationType)), isInContrastMode);
        }

        public void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            EnsurePriorityOrder();
            if (_priorityOrder.Contains(classificationType))
            {
                SetExplicitTextProperties(classificationType, properties);
            }
            else
            {
                _priorityOrder.Add(classificationType);
                StoreProperties(classificationType, properties);
            }
        }

        public void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties, IClassificationType priority)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (priority == null)
                throw new ArgumentNullException(nameof(priority));
            EnsurePriorityOrder();
            if (_priorityOrder.Contains(classificationType))
            {
                SetExplicitTextProperties(classificationType, properties);
            }
            else
            {
                var index = _priorityOrder.IndexOf(priority);
                if (index == -1)
                    throw new KeyNotFoundException(nameof(priority));
                _priorityOrder.Insert(index, classificationType);
                StoreProperties(classificationType, properties);
            }
        }

        public void SetTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (properties == TextFormattingRunProperties.CreateTextFormattingRunProperties())
            {
                StoreProperties(classificationType, new ResourceDictionary());
            }
            else
            {
                var classificationTypeList = new List<IClassificationType>();
                BuildOrderedClassifications(classificationType, classificationTypeList);
                classificationTypeList.RemoveAt(0);
                var resourceDictionary = MergeProperties(classificationTypeList);
                var properties1 = new ResourceDictionary();
                var rdFromTfrp = CreateRdFromTfrp(properties);
                foreach (string key in rdFromTfrp.Keys)
                {
                    var flag = false;
                    if (resourceDictionary.Contains(key))
                    {
                        var obj = rdFromTfrp[key];
                        var typeface = obj as Typeface;
                        var brush = obj as Brush;
                        flag = brush == null
                            ? (typeface == null
                                ? obj.Equals(resourceDictionary[key])
                                : WpfHelper.TypefacesEqual(typeface, resourceDictionary[key] as Typeface))
                            : WpfHelper.BrushesEqual(brush, resourceDictionary[key] as Brush);
                    }
                    if (!flag)
                        properties1[key] = rdFromTfrp[key];
                }
                StoreProperties(classificationType, properties1);
            }
        }

        public void SetExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties)
        {
            if (classificationType == null)
                throw new ArgumentNullException(nameof(classificationType));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            StoreProperties(classificationType, properties);
        }

        public void BeginBatchUpdate()
        {
            if (!_formatMap.IsInBatchUpdate)
                _formatMap.BeginBatchUpdate();
            if (_raiseEventOnBatchEnd.HasValue)
                throw new InvalidOperationException("BeginBatchUpdate called twice without calling EndBatchUpdate");
            _raiseEventOnBatchEnd = false;
        }

        public void EndBatchUpdate()
        {
            if (!_raiseEventOnBatchEnd.HasValue)
                throw new InvalidOperationException("EndBatchUpdate called without BeginBatchUpdate being called");
            if (_raiseEventOnBatchEnd.Value)
                RaiseChangedEvent();
            _formatMap.EndBatchUpdate();
            _raiseEventOnBatchEnd = new bool?();
        }

        public bool IsInBatchUpdate => _raiseEventOnBatchEnd.HasValue;

        private void EnsureDefaultTextProperties()
        {
            if (!_defaultTextProperties.Contains("Typeface"))
                _defaultTextProperties["Typeface"] = new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, new FontFamily("Global Monospace, Global User Interface"));
            if (!_defaultTextProperties.Contains("Foreground"))
                _defaultTextProperties["Foreground"] = SystemColors.WindowTextBrush;
            if (!_defaultTextProperties.Contains("FontRenderingSize"))
                _defaultTextProperties["FontRenderingSize"] = 16.0;
            _defaultTextFormattingRunProperties = CreateTfrpFromRd(_defaultTextProperties, _optionsService.GlobalOptions.IsInContrastMode());
        }

        private void EnsurePriorityOrder()
        {
            if (_priorityOrder != null)
                return;
            _priorityOrder = new List<IClassificationType>();
            CreateFormatPriorityOrder(Orderer.Order(Formats));
        }

        private void OnFormatMappingChanged(object sender, EventArgs e)
        {
            RecordStateChange();
        }

        private void RaiseChangedEvent()
        {
            _textFormattingPropertiesWithDefaults.Clear();
            _formatMap.FormatMappingChanged -= OnFormatMappingChanged;
            var formatMappingChanged = ClassificationFormatMappingChanged;
            formatMappingChanged?.Invoke(this, new EventArgs());
            _formatMap.FormatMappingChanged += OnFormatMappingChanged;
        }

        private void CreateFormatPriorityOrder(IEnumerable<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> orders)
        {
            foreach (var order in orders)
            {
                var name = ((IEditorFormatMetadata)order.Metadata).Name;
                if (name != "Low Priority" && name != "Default Priority" && name != "High Priority")
                {
                    foreach (var classificationTypeName in order.Metadata.ClassificationTypeNames)
                        _priorityOrder.Add(ClassificationTypeRegistry.GetClassificationType(classificationTypeName));
                }
            }
        }

        private ResourceDictionary GetResourceDictionaryFromHierarchy(IClassificationType classificationType)
        {
            var classificationTypeList = new List<IClassificationType>();
            EnsurePriorityOrder();
            BuildOrderedClassifications(classificationType, classificationTypeList);
            return MergeProperties(classificationTypeList);
        }

        private ResourceDictionary MergeProperties(IReadOnlyList<IClassificationType> classificationTypes)
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.MergedDictionaries.Add(_defaultTextProperties);
            for (var index = classificationTypes.Count - 1; index >= 0; --index)
                resourceDictionary.MergedDictionaries.Add(GetRdFromClassification(classificationTypes[index]));
            return resourceDictionary;
        }

        private void BuildOrderedClassifications(IClassificationType classificationType, List<IClassificationType> orderedClassifications)
        {
            if (orderedClassifications.Contains(classificationType))
                return;
            orderedClassifications.Add(classificationType);
            var classificationTypeList = new List<IClassificationType>(classificationType.BaseTypes);
            if (classificationTypeList.Count > 1)
                classificationTypeList.Sort((p1, p2) => _priorityOrder.IndexOf(p2).CompareTo(_priorityOrder.IndexOf(p1)));
            foreach (var classificationType1 in classificationTypeList)
                BuildOrderedClassifications(classificationType1, orderedClassifications);
        }

        private void RecordStateChange()
        {
            if (IsInBatchUpdate)
                MarkAsDirty();
            else
                RaiseChangedEvent();
        }

        internal string GetFormatNameFromClassification(string classification)
        {
            if (_classificationToFormatNameMap.TryGetValue(classification, out var str))
                return str;
            foreach (var format in Formats)
            {
                if (format.Metadata.ClassificationTypeNames.Any(classificationTypeName => string.Compare(classificationTypeName, classification, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    _classificationToFormatNameMap[classification] = ((IEditorFormatMetadata)format.Metadata).Name;
                    return ((IEditorFormatMetadata)format.Metadata).Name;
                }
            }
            return null;
        }

        private void StoreProperties(IClassificationType classificationType, TextFormattingRunProperties properties)
        {
            StoreProperties(classificationType, CreateRdFromTfrp(properties));
        }

        private void StoreProperties(IClassificationType classificationType, ResourceDictionary properties)
        {
            if (IsInBatchUpdate)
                MarkAsDirty();
            _formatMap.SetProperties(GetEditorFormatMapKey(classificationType), properties);
        }

        private ResourceDictionary GetRdFromClassification(IClassificationType classificationType)
        {
            return _formatMap.GetProperties(GetEditorFormatMapKey(classificationType));
        }

        private static TextFormattingRunProperties CreateTfrpFromRd(ResourceDictionary dictionary, bool isInContrastMode)
        {
            var size = new double?();
            var hintingSize = new double?();
            var foreground = dictionary["Foreground"] as Brush;
            if (foreground == null && dictionary.Contains("ForegroundColor"))
                foreground = new SolidColorBrush((Color)dictionary["ForegroundColor"]);
            if (foreground != null && dictionary.Contains("ForegroundOpacity"))
            {
                var num = (double)dictionary["ForegroundOpacity"];
                foreground = foreground.Clone();
                if (!isInContrastMode)
                    foreground.Opacity = num;
            }
            var background = dictionary["Background"] as Brush;
            if (background == null && dictionary.Contains("BackgroundColor"))
                background = new SolidColorBrush((Color)dictionary["BackgroundColor"]);
            if (background != null)
            {
                if (background.IsFrozen)
                    background = background.Clone();
                if (!isInContrastMode)
                    background.Opacity = !dictionary.Contains("BackgroundOpacity") ? 0.8 : (double)dictionary["BackgroundOpacity"];
            }
            var nullable1 = dictionary["IsBold"] as bool?;
            var nullable2 = dictionary["IsItalic"] as bool?;
            var typeface = dictionary["Typeface"] as Typeface;
            if (typeface != null)
            {
                var weight = nullable1.HasValue ? (nullable1.Value ? FontWeights.Bold : FontWeights.Normal) : typeface.Weight;
                var style = nullable2.HasValue ? (nullable2.Value ? FontStyles.Italic : FontStyles.Normal) : typeface.Style;
                if (weight != typeface.Weight || style != typeface.Style)
                    typeface = new Typeface(typeface.FontFamily, style, weight, typeface.Stretch, new FontFamily("Global Monospace, Global User Interface"));
            }
            if (dictionary.Contains("FontRenderingSize"))
                size = (double)dictionary["FontRenderingSize"];
            if (dictionary.Contains("FontHintingSize"))
                hintingSize = (double)dictionary["FontHintingSize"];
            var textDecorations = dictionary["TextDecorations"] as TextDecorationCollection;
            var textEffects = dictionary["TextEffects"] as TextEffectCollection;
            var cultureInfo = dictionary["CultureInfo"] as CultureInfo;
            var formattingRunProperties = TextFormattingRunProperties.CreateTextFormattingRunProperties(foreground, background, typeface, size, hintingSize, textDecorations, textEffects, cultureInfo);
            if (foreground == null)
            {
                if (dictionary["ForegroundOpacity"] is double nullable3)
                    formattingRunProperties = formattingRunProperties.SetForegroundOpacity(nullable3);
            }
            if (background == null)
            {
                if (dictionary["BackgroundOpacity"] is double nullable3)
                    formattingRunProperties = formattingRunProperties.SetBackgroundOpacity(nullable3);
            }
            if (nullable1.HasValue)
                formattingRunProperties = formattingRunProperties.SetBold(nullable1.Value);
            if (nullable2.HasValue)
                formattingRunProperties = formattingRunProperties.SetItalic(nullable2.Value);
            return formattingRunProperties;
        }

        private void MarkAsDirty()
        {
            _raiseEventOnBatchEnd = true;
        }

        private static ResourceDictionary CreateRdFromTfrp(TextFormattingRunProperties properties)
        {
            var resourceDictionary = new ResourceDictionary();
            if (!properties.ForegroundBrushEmpty)
            {
                resourceDictionary.Add("Foreground", properties.ForegroundBrush);
                if (properties.ForegroundBrush.Opacity < 1.0)
                    resourceDictionary.Add("ForegroundOpacity", properties.ForegroundBrush.Opacity);
            }
            if (!properties.ForegroundOpacityEmpty)
                resourceDictionary["ForegroundOpacity"] = properties.ForegroundOpacity;
            if (!properties.BackgroundBrushEmpty)
            {
                resourceDictionary.Add("Background", properties.BackgroundBrush);
                if (properties.BackgroundBrush.Opacity < 1.0)
                    resourceDictionary.Add("BackgroundOpacity", properties.BackgroundBrush.Opacity);
            }
            if (!properties.BackgroundOpacityEmpty)
                resourceDictionary["BackgroundOpacity"] = properties.BackgroundOpacity;
            if (!properties.TypefaceEmpty)
                resourceDictionary.Add("Typeface", properties.Typeface);
            if (!properties.BoldEmpty)
                resourceDictionary.Add("IsBold", properties.Bold);
            if (!properties.ItalicEmpty)
                resourceDictionary.Add("IsItalic", properties.Italic);
            if (!properties.FontRenderingEmSizeEmpty)
                resourceDictionary.Add("FontRenderingSize", properties.FontRenderingEmSize);
            if (!properties.FontHintingEmSizeEmpty)
                resourceDictionary.Add("FontHintingSize", properties.FontHintingEmSize);
            if (!properties.TextDecorationsEmpty)
                resourceDictionary.Add("TextDecorations", properties.TextDecorations);
            if (!properties.TextEffectsEmpty)
                resourceDictionary.Add("TextEffects", properties.TextEffects);
            if (!properties.CultureInfoEmpty)
                resourceDictionary.Add("CultureInfo", properties.CultureInfo);
            return resourceDictionary;
        }
    }
}