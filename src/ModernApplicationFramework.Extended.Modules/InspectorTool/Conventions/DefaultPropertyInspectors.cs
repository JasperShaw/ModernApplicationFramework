using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Conventions
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

                new StandardPropertyEditorBuilder<bool, CheckBoxEditorViewModel>(),
                new StandardPropertyEditorBuilder<double, TextBoxEditorViewModel<double>>(),
                new StandardPropertyEditorBuilder<float, TextBoxEditorViewModel<float>>(),
                new StandardPropertyEditorBuilder<int, TextBoxEditorViewModel<int>>(),
                new StandardPropertyEditorBuilder<double?, TextBoxEditorViewModel<double?>>(),
                new StandardPropertyEditorBuilder<float?, TextBoxEditorViewModel<float?>>(),
                new StandardPropertyEditorBuilder<int?, TextBoxEditorViewModel<int?>>(),
                new StandardPropertyEditorBuilder<string, TextBoxEditorViewModel<string>>(),

                new StandardPropertyEditorBuilder<BitmapSource, BitmapSourceEditorViewModel>()
            };
        }

        public static IEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            foreach (var inspectorBuilder in InspectorBuilders)
                if (inspectorBuilder.IsApplicable(propertyDescriptor))
                    return inspectorBuilder.BuildEditor(propertyDescriptor);
            return null;
        }
    }
}