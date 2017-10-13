using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.KeyBindingScheme;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.FileKeySchemes
{
    [Export(typeof(IKeyBindingSchemeManager))]
    public sealed class FileKeySchemeManager : AbstractKeyBindingSchemeManager
    {
        public override void LoadSchemeDefinitions()
        {
            var env = IoC.Get<IExtendedEnvironmentVariables>();
            env.GetEnvironmentVariable(env.ApplicationUserDirectoryKey, out var path);  
            var files = Directory.GetFiles(Environment.ExpandEnvironmentVariables(path), "*.mafk", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return;

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                SchemeDefinitions.Add(new DefaultSchemeDefinition(fileName, file));
            }
        }

        public override void SetDefaultScheme()
        {
            var settings = IoC.Get<KeyBindingsSettings>();
            var name = settings.KeyboardShortcuts.ShortcutsScheme;
            SetScheme(SchemeDefinitions.LastOrDefault(x => x.Name == name));
        }
    }

    public class DefaultSchemeDefinition : FileSchemeDefinition
    {
        public DefaultSchemeDefinition(string name, string fileLocation) : base(name, fileLocation)
        {
        }

        public override KeyBindingScheme Load()
        {
            return KeyBindingScheme.LoadFromFile(FileLocation);
        }
    }
}
