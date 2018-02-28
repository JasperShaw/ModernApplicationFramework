﻿using System.Collections.Generic;
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
                    return inspectorBuilder.BuildEditor(propertyDescriptor);
            return null;
        }
    }
}