using ModernApplicationFramework.Extended.KeyBindingScheme;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IKeyBindingManager
    {
        IKeyBindingSchemeManager SchemeManager { get; }

        IKeyGestureService KeyGestureService { get; }

        void BuildCurrent();

        void SaveCurrent();

        void ApplyKeyBindingsFromSettings();

        void ResetToKeyScheme(SchemeDefinition selectedScheme);

        void SetKeyScheme(SchemeDefinition selectedScheme);

        void LoadDefaultScheme();
    }
}