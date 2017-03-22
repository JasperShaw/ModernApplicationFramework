using System.Reflection;
using System.Runtime.InteropServices;
// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Modern Application Framework")]
[assembly: AssemblyDescription("This assembly contains some Controls that are styled like controls from Visual Studio are.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Modern Application Framework")]
[assembly: AssemblyCopyright("Copyright (C) Anakin_Sklavenwalker")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten.  Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("0b79114d-2845-481b-b7cf-d5344e802ffc")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, 
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: XmlnsPrefix("http://modern.framework.com/controls", "mafc")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.Controls")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.Controls.Utilities")]
[assembly: XmlnsPrefix("http://modern.framework.com/command", "mafcb")]
[assembly: XmlnsDefinition("http://modern.framework.com/command", "ModernApplicationFramework.CommandBase")]
[assembly: XmlnsDefinition("http://modern.framework.com/command", "ModernApplicationFramework.CommandBase.Base")]
[assembly: XmlnsPrefix("http://modern.framework.com/basics", "mafb")]
[assembly: XmlnsDefinition("http://modern.framework.com/basics", "ModernApplicationFramework.Basics")]
[assembly: XmlnsDefinition("http://modern.framework.com/basics", "ModernApplicationFramework.Basics.ViewModels")]
[assembly: XmlnsPrefix("http://modern.framework.com/docking", "mafd")]
[assembly: XmlnsDefinition("http://modern.framework.com/docking", "ModernApplicationFramework.Docking")]
[assembly: XmlnsDefinition("http://modern.framework.com/docking", "ModernApplicationFramework.Docking.Controls")]
[assembly: XmlnsDefinition("http://modern.framework.com/docking", "ModernApplicationFramework.Docking.Converters")]
[assembly: XmlnsDefinition("http://modern.framework.com/docking", "ModernApplicationFramework.Docking.Layout")]


