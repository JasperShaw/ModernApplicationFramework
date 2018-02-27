﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die einer Assembly zugeordnet sind.
[assembly: AssemblyTitle("ModernApplicationFramework.MVVM")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ModernApplicationFramework.MVVM")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten.  Wenn Sie auf einen Typ in dieser Assembly von 
// COM aus zugreifen müssen, sollten Sie das ComVisible-Attribut für diesen Typ auf "True" festlegen.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("1f020157-699c-4e63-80ca-72d62191e063")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.MVVM.Core")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.MVVM.Core.Utilities")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.MVVM.Controls")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.MVVM.Interfaces")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.MVVM.ViewModels")]
[assembly: XmlnsDefinition("http://modern.framework.com/controls", "ModernApplicationFramework.MVVM.Views")]