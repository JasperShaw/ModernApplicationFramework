﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModernApplicationFramework.Extended.Settings.General {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class GeneralSettingsResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal GeneralSettingsResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ModernApplicationFramework.Extended.Settings.General.GeneralSettingsResources", typeof(GeneralSettingsResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auto Hide button affects active tool window only.
        /// </summary>
        public static string AutoHideAffectsOnlyActiveCheckBox_Text {
            get {
                return ResourceManager.GetString("AutoHideAffectsOnlyActiveCheckBox_Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Close button affects active tool window only.
        /// </summary>
        public static string CloseAffectsOnlyActiveCheckBox_Text {
            get {
                return ResourceManager.GetString("CloseAffectsOnlyActiveCheckBox_Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Window menu can contain {0} - {1} items..
        /// </summary>
        public static string Error_ItemsListCountNotMatching {
            get {
                return ResourceManager.GetString("Error_ItemsListCountNotMatching", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to General.
        /// </summary>
        public static string GeneralSettings_Name {
            get {
                return ResourceManager.GetString("GeneralSettings_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show status bar.
        /// </summary>
        public static string UseStatusBarCheckBox_Text {
            get {
                return ResourceManager.GetString("UseStatusBarCheckBox_Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to items shown in _Window menu.
        /// </summary>
        public static string WindowItemsTextBox_Text {
            get {
                return ResourceManager.GetString("WindowItemsTextBox_Text", resourceCulture);
            }
        }
    }
}