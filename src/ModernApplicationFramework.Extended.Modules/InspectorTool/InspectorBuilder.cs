using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Conventions;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Util;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public class InspectorBuilder<TBuilder> where TBuilder : InspectorBuilder<TBuilder>
    {
        protected List<IInspector> Inspectors { get; }

        public InspectorBuilder()
        {
            Inspectors = new List<IInspector>();
        }

        public TBuilder WithCollapsibleGroup(string name,
            Func<CollapsibleGroupBuilder, CollapsibleGroupBuilder> callback)
        {
            var builder = new CollapsibleGroupBuilder();
            Inspectors.Add(callback(builder).ToCollapsibleGroup(name));
            return (TBuilder) this;
        }

        public TBuilder WithCheckBoxEditor<T>(T instance, Expression<Func<T, bool>> propertyExpression)
        {
            return WithEditor<T, bool, CheckBoxInspectorEditorViewModel>(instance, propertyExpression);
        }

        public TBuilder WithEnumEditor<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyExpression)
        {
            return WithEditor<T, TProperty, EnumInspectorEditorViewModel<TProperty>>(instance, propertyExpression);
        }


        public TBuilder WithEditor<T, TProperty, TEditor>(T instance, Expression<Func<T, TProperty>> propertyExpression)
            where TEditor : IInspectorEditor, new()
        {
            return WithEditor(instance, propertyExpression, new TEditor());
        }

        public TBuilder WithEditor<T, TProperty, TEditor>(T instance, Expression<Func<T, TProperty>> propertyExpression,
            TEditor editor)
            where TEditor : IInspectorEditor
        {
            var propertyName = ExpressionUtility.GetPropertyName(propertyExpression);
            editor.BoundPropertyDescriptor = BoundPropertyDescriptor.FromProperty(instance, propertyName);
            Inspectors.Add(editor);
            return (TBuilder) this;
        }

        public TBuilder WithObjectProperties(object instance, Func<PropertyDescriptor, bool> propertyFilter)
        {
            var properties = TypeDescriptor.GetProperties(instance)
                .Cast<PropertyDescriptor>()
                .Where(x => x.IsBrowsable && propertyFilter(x))
                .ToList();

            // If any properties are not in the default group, show all properties in collapsible groups.
            if (properties.Any(x => !string.IsNullOrEmpty(x.Category) &&
                                    x.Category != CategoryAttribute.Default.Category))
                foreach (var category in properties.GroupBy(x => x.Category))
                {
                    var actualCategory = string.IsNullOrEmpty(category.Key) ||
                                         category.Key == CategoryAttribute.Default.Category
                        ? "Miscellaneous"
                        : category.Key;

                    var collapsibleGroupBuilder = new CollapsibleGroupBuilder();
                    AddProperties(instance, category, collapsibleGroupBuilder.Inspectors);
                    if (collapsibleGroupBuilder.Inspectors.Any())
                        Inspectors.Add(collapsibleGroupBuilder.ToCollapsibleGroup(actualCategory));
                }
            else // Otherwise, show properties in flat list.
                AddProperties(instance, properties, Inspectors);

            return (TBuilder) this;
        }

        private static void AddProperties(object instance, IEnumerable<PropertyDescriptor> properties,
            List<IInspector> inspectors)
        {
            foreach (var property in properties)
            {
                var editor = DefaultPropertyInspectors.CreateEditor(property);
                if (editor != null)
                {
                    editor.BoundPropertyDescriptor = new BoundPropertyDescriptor(instance, property);
                    inspectors.Add(editor);
                }
            }
        }
    }
}