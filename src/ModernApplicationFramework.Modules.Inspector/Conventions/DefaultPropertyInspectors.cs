using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Modules.Inspector.Inspectors;

namespace ModernApplicationFramework.Modules.Inspector.Conventions
{
    public static class DefaultPropertyInspectors
    {
        // ReSharper disable once InconsistentNaming

        public static List<PropertyEditorBuilder> InspectorBuilders { get; }

        static DefaultPropertyInspectors()
        {
            InspectorBuilders = new List<PropertyEditorBuilder>
            {
                new EnumPropertyEditorBuilder(),

                new StandardPropertyEditorBuilder<bool, CheckBoxInspectorEditorViewModel>(),
                new StandardPropertyEditorBuilder<double, TextBoxInspectorEditorViewModel<double>>(),
                new StandardPropertyEditorBuilder<float, TextBoxInspectorEditorViewModel<float>>(),
                new StandardPropertyEditorBuilder<int, TextBoxInspectorEditorViewModel<int>>(),
                new StandardPropertyEditorBuilder<double?, TextBoxInspectorEditorViewModel<double?>>(),
                new StandardPropertyEditorBuilder<float?, TextBoxInspectorEditorViewModel<float?>>(),
                new StandardPropertyEditorBuilder<int?, TextBoxInspectorEditorViewModel<int?>>(),
                new StandardPropertyEditorBuilder<string, TextBoxInspectorEditorViewModel<string>>(),

                new StandardPropertyEditorBuilder<BitmapSource, BitmapSourceInspectorEditorViewModel>()
            };
        }

        public static IInspectorEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            foreach (var inspectorBuilder in InspectorBuilders)
                if (inspectorBuilder.IsApplicable(propertyDescriptor))
                {
                    var attribute = propertyDescriptor.Attributes.OfType<IgnorePropertyAttribute>().FirstOrDefault();
                    if (attribute != null && attribute.IgnoreProperty)
                        return null;
                    return inspectorBuilder.BuildEditor(propertyDescriptor);
                }
            return null;
        }
    }
}