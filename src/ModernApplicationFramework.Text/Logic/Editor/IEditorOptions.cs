using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public interface IEditorOptions
    {
        T GetOptionValue<T>(string optionId);

        T GetOptionValue<T>(EditorOptionKey<T> key);

        object GetOptionValue(string optionId);

        void SetOptionValue(string optionId, object value);

        void SetOptionValue<T>(EditorOptionKey<T> key, T value);

        bool IsOptionDefined(string optionId, bool localScopeOnly);

        bool IsOptionDefined<T>(EditorOptionKey<T> key, bool localScopeOnly);

        bool ClearOptionValue(string optionId);

        bool ClearOptionValue<T>(EditorOptionKey<T> key);

        IEnumerable<EditorOptionDefinition> SupportedOptions { get; }

        IEditorOptions GlobalOptions { get; }

        IEditorOptions Parent { get; set; }

        event EventHandler<EditorOptionChangedEventArgs> OptionChanged;
    }
}