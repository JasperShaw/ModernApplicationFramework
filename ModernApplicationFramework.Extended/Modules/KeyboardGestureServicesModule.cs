using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.ModuleBase;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Modules
{
    /// <summary>
    /// A <see cref="ModuleBase"/> that initializes the an <see cref="IKeyboardInputService"/> and an <see cref="IKeyBindingSchemeManager"/>. 
    /// Calls the <see cref="IKeyBindingSchemeManager.SetDefaultScheme"/> methods afterwards.
    /// </summary>
    /// <seealso cref="ModuleBase" />
    [Export(typeof(IModule))]
    [Export(typeof(KeyboardGestureServicesModule))]
    public class KeyboardGestureServicesModule : ModuleBase
    {
        private readonly IKeyGestureService _gestureService;
        private readonly IKeyBindingSchemeManager _schemeManager;

        [ImportingConstructor]
        public KeyboardGestureServicesModule(IKeyGestureService gestureService, IKeyBindingSchemeManager schemeManager)
        {
            _gestureService = gestureService;
            _schemeManager = schemeManager;
        }
        
        public override void PreInitialize()
        {
            _gestureService.Initialize();
            _schemeManager.LoadSchemeDefinitions();
            _schemeManager.SetDefaultScheme();
        }
    }
}