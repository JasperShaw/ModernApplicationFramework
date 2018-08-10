using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public interface IEditorOptions
    {
        event EventHandler<EditorOptionChangedEventArgs> OptionChanged;

        IEditorOptions GlobalOptions { get; }

        IEditorOptions Parent { get; set; }

        IEnumerable<EditorOptionDefinition> SupportedOptions { get; }

        bool ClearOptionValue(string optionId);

        bool ClearOptionValue<T>(EditorOptionKey<T> key);
        T GetOptionValue<T>(string optionId);

        T GetOptionValue<T>(EditorOptionKey<T> key);

        object GetOptionValue(string optionId);

        bool IsOptionDefined(string optionId, bool localScopeOnly);

        bool IsOptionDefined<T>(EditorOptionKey<T> key, bool localScopeOnly);

        void SetOptionValue(string optionId, object value);

        void SetOptionValue<T>(EditorOptionKey<T> key, T value);
    }
}