using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Conventions
{
    public static class DefaultPropertyInspectors
    {
        // ReSharper disable once InconsistentNaming
        private static readonly List<PropertyEditorBuilder> _inspectorBuilders;

        public static List<PropertyEditorBuilder> InspectorBuilders => _inspectorBuilders;

        static DefaultPropertyInspectors()
        {
            _inspectorBuilders = new List<PropertyEditorBuilder>
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
            foreach (var inspectorBuilder in _inspectorBuilders)
            {
                if (inspectorBuilder.IsApplicable(propertyDescriptor))
                    return inspectorBuilder.BuildEditor(propertyDescriptor);
            }
            return null;
        }
    }
}
