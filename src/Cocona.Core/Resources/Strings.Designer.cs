﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cocona.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cocona.Resources.Strings", typeof(Strings).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generate a shell completion code.
        /// </summary>
        internal static string BuiltIn_Command_Completion_Description {
            get {
                return ResourceManager.GetString("BuiltIn_Command_Completion_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: Shell completion for &apos;{0}&apos; is not supported. (Supported shells: {1}).
        /// </summary>
        internal static string BuiltIn_Command_Completion_Error_NotSupported {
            get {
                return ResourceManager.GetString("BuiltIn_Command_Completion_Error_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generate a shell completion candidates.
        /// </summary>
        internal static string BuiltIn_Command_CompletionCanditates_Description {
            get {
                return ResourceManager.GetString("BuiltIn_Command_CompletionCanditates_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show help message.
        /// </summary>
        internal static string BuiltIn_Command_Help_Description {
            get {
                return ResourceManager.GetString("BuiltIn_Command_Help_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show version.
        /// </summary>
        internal static string BuiltIn_Command_Version_Description {
            get {
                return ResourceManager.GetString("BuiltIn_Command_Version_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: Argument &apos;{0}&apos; is required. See &apos;--help&apos; for usage..
        /// </summary>
        internal static string Command_Error_Insufficient_Argument {
            get {
                return ResourceManager.GetString("Command_Error_Insufficient_Argument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: Option &apos;--{0}&apos; is required. See &apos;--help&apos; for usage..
        /// </summary>
        internal static string Command_Error_Insufficient_Option {
            get {
                return ResourceManager.GetString("Command_Error_Insufficient_Option", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: Option &apos;--{0}&apos; requires a value. See &apos;--help&apos; for usage..
        /// </summary>
        internal static string Command_Error_Insufficient_OptionValue {
            get {
                return ResourceManager.GetString("Command_Error_Insufficient_OptionValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: {0}.
        /// </summary>
        internal static string Command_Error_ParameterBind {
            get {
                return ResourceManager.GetString("Command_Error_ParameterBind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: Unknown option &apos;{0}&apos;.
        /// </summary>
        internal static string Command_Error_UnknownOption {
            get {
                return ResourceManager.GetString("Command_Error_UnknownOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command not yet implemented..
        /// </summary>
        internal static string Command_NotYetImplemented {
            get {
                return ResourceManager.GetString("Command_NotYetImplemented", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: {0}.
        /// </summary>
        internal static string Dispatcher_Error_CommandNotFound {
            get {
                return ResourceManager.GetString("Dispatcher_Error_CommandNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: &apos;{0}&apos; is not a command. See &apos;--help&apos; for usage..
        /// </summary>
        internal static string Dispatcher_Error_NotACommand {
            get {
                return ResourceManager.GetString("Dispatcher_Error_NotACommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Similar commands:.
        /// </summary>
        internal static string Dispatcher_Error_SimilarCommands {
            get {
                return ResourceManager.GetString("Dispatcher_Error_SimilarCommands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command has exited with code &apos;{0}&apos;.{1}.
        /// </summary>
        internal static string Exception_TheCommandHasExitedWithCode {
            get {
                return ResourceManager.GetString("Exception_TheCommandHasExitedWithCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Allowed values.
        /// </summary>
        internal static string Help_Description_AllowedValues {
            get {
                return ResourceManager.GetString("Help_Description_AllowedValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Default.
        /// </summary>
        internal static string Help_Description_Default {
            get {
                return ResourceManager.GetString("Help_Description_Default", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Required.
        /// </summary>
        internal static string Help_Description_Required {
            get {
                return ResourceManager.GetString("Help_Description_Required", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arguments:.
        /// </summary>
        internal static string Help_Heading_Arguments {
            get {
                return ResourceManager.GetString("Help_Heading_Arguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Commands:.
        /// </summary>
        internal static string Help_Heading_Commands {
            get {
                return ResourceManager.GetString("Help_Heading_Commands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Options:.
        /// </summary>
        internal static string Help_Heading_Options {
            get {
                return ResourceManager.GetString("Help_Heading_Options", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usage: {0}.
        /// </summary>
        internal static string Help_Index_Usage {
            get {
                return ResourceManager.GetString("Help_Index_Usage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usage: {0} {1}[command].
        /// </summary>
        internal static string Help_Index_Usage_Multple {
            get {
                return ResourceManager.GetString("Help_Index_Usage_Multple", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        internal static string Markdown_Heading_ApplicationDescription {
            get {
                return ResourceManager.GetString("Markdown_Heading_ApplicationDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arguments.
        /// </summary>
        internal static string Markdown_Heading_Arguments {
            get {
                return ResourceManager.GetString("Markdown_Heading_Arguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        internal static string Markdown_Heading_CommandDescription {
            get {
                return ResourceManager.GetString("Markdown_Heading_CommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Options.
        /// </summary>
        internal static string Markdown_Heading_Options {
            get {
                return ResourceManager.GetString("Markdown_Heading_Options", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command structure.
        /// </summary>
        internal static string Markdown_Heading_Usage {
            get {
                return ResourceManager.GetString("Markdown_Heading_Usage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A method of option-like command &apos;{0}&apos; was not found in &apos;{1}&apos;.
        /// </summary>
        internal static string OptionLikeCommand_MethodNotFound {
            get {
                return ResourceManager.GetString("OptionLikeCommand_MethodNotFound", resourceCulture);
            }
        }
    }
}
