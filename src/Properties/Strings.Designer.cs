﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wheatech.Modulize.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Wheatech.Modulize.Properties.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
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
        ///   查找类似 There are multiple types implemented IFeatureActivator interface and the FeatureId returns the same value &quot;{0}&quot; in the assembly {1}. 的本地化字符串。
        /// </summary>
        internal static string Activation_AmbiguousExplicitFeatureActivator {
            get {
                return ResourceManager.GetString("Activation_AmbiguousExplicitFeatureActivator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 There are multiple types implemented IFeatureActivator interface without FeatureId in the assembly {0}. The application cannot determine which one should be used. 的本地化字符串。
        /// </summary>
        internal static string Activation_AmbiguousImplictFeatureActivator {
            get {
                return ResourceManager.GetString("Activation_AmbiguousImplictFeatureActivator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The parameter {0} of the method {1} declared in the type {2} has not been registered. 的本地化字符串。
        /// </summary>
        internal static string Activation_CannotFindParameter {
            get {
                return ResourceManager.GetString("Activation_CannotFindParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The method {0} in the type {1} cannot be generic. 的本地化字符串。
        /// </summary>
        internal static string Activation_CannotGenericMethod {
            get {
                return ResourceManager.GetString("Activation_CannotGenericMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The entry assembly of the feature {1} cannot be loaded: {0}. 的本地化字符串。
        /// </summary>
        internal static string Activation_CannotLoadFeatureEntry {
            get {
                return ResourceManager.GetString("Activation_CannotLoadFeatureEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The entry assembly of the module {1} cannot be loaded: {0}. 的本地化字符串。
        /// </summary>
        internal static string Activation_CannotLoadModuleEntry {
            get {
                return ResourceManager.GetString("Activation_CannotLoadModuleEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 There are multiple public method {0} have been found in the type {1}. The application cannot determine which one should be used to invoke. 的本地化字符串。
        /// </summary>
        internal static string Activation_CannotMultipleMethod {
            get {
                return ResourceManager.GetString("Activation_CannotMultipleMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The circle dependency has been detected for the features: {0}. 的本地化字符串。
        /// </summary>
        internal static string Activation_CircleDependency {
            get {
                return ResourceManager.GetString("Activation_CircleDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 There are multiple features with the same ID: {0}. 的本地化字符串。
        /// </summary>
        internal static string Activation_DuplicateFeatures {
            get {
                return ResourceManager.GetString("Activation_DuplicateFeatures", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 There are multiple modules with the same ID: {0}. 的本地化字符串。
        /// </summary>
        internal static string Activation_DuplicateModules {
            get {
                return ResourceManager.GetString("Activation_DuplicateModules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The parameter {0} of the method {1} declared in the type {2} cannot be out or ref. 的本地化字符串。
        /// </summary>
        internal static string Activation_InvalidParameter {
            get {
                return ResourceManager.GetString("Activation_InvalidParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The property {0} declared in the type {1} cannot have index parameters. 的本地化字符串。
        /// </summary>
        internal static string Activation_PropertyCannotBeIndexer {
            get {
                return ResourceManager.GetString("Activation_PropertyCannotBeIndexer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The property {0} declared in the type {1} does not have getter method. 的本地化字符串。
        /// </summary>
        internal static string Activation_PropertyMustCanRead {
            get {
                return ResourceManager.GetString("Activation_PropertyMustCanRead", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The return value of property {0} declared in the type {1} must be string. 的本地化字符串。
        /// </summary>
        internal static string Activation_PropertyMustReturnString {
            get {
                return ResourceManager.GetString("Activation_PropertyMustReturnString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Value cannot be null or an empty string. 的本地化字符串。
        /// </summary>
        internal static string Argument_Cannot_Be_Null_Or_Empty {
            get {
                return ResourceManager.GetString("Argument_Cannot_Be_Null_Or_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The collection is read only. 的本地化字符串。
        /// </summary>
        internal static string Collection_ReadOnly {
            get {
                return ResourceManager.GetString("Collection_ReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The module container has not been started. 的本地化字符串。
        /// </summary>
        internal static string Container_NotStart {
            get {
                return ResourceManager.GetString("Container_NotStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Ambiguous assemblies have been found for the entry assembly of the feature {0}. 的本地化字符串。
        /// </summary>
        internal static string Discover_AmbiguousFeatureEntry {
            get {
                return ResourceManager.GetString("Discover_AmbiguousFeatureEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Ambiguous assemblies have been found for the entry assembly of the module {0}. 的本地化字符串。
        /// </summary>
        internal static string Discover_AmbiguousModuleEntry {
            get {
                return ResourceManager.GetString("Discover_AmbiguousModuleEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The entry assembly for the feature {0} cannot be found: {1} 的本地化字符串。
        /// </summary>
        internal static string Discover_CannotFindFeatureEntry {
            get {
                return ResourceManager.GetString("Discover_CannotFindFeatureEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The entry assembly for the module {0} cannot be found: {1} 的本地化字符串。
        /// </summary>
        internal static string Discover_CannotFindModuleEntry {
            get {
                return ResourceManager.GetString("Discover_CannotFindModuleEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &apos;{0}&apos; is not a valid dependency string. 的本地化字符串。
        /// </summary>
        internal static string Invalid_Dependency {
            get {
                return ResourceManager.GetString("Invalid_Dependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The json token {0} is not supported in the file {1}. 的本地化字符串。
        /// </summary>
        internal static string Locator_InvalidJsonToken {
            get {
                return ResourceManager.GetString("Locator_InvalidJsonToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The format is invalid at line {0} in the file {1}. 的本地化字符串。
        /// </summary>
        internal static string Locator_InvalidTextLine {
            get {
                return ResourceManager.GetString("Locator_InvalidTextLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &apos;{0}&apos; is not a valid dependency in the manifest file of the module {1}. 的本地化字符串。
        /// </summary>
        internal static string Manifest_InvalidDependency {
            get {
                return ResourceManager.GetString("Manifest_InvalidDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &apos;{0}&apos; is not a valid dependency version in the manifest file of the module {1}. 的本地化字符串。
        /// </summary>
        internal static string Manifest_InvalidDependencyVersion {
            get {
                return ResourceManager.GetString("Manifest_InvalidDependencyVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The format of {0} is invalid in the manifest file of the module {1}. 的本地化字符串。
        /// </summary>
        internal static string Manifest_InvalidFormat {
            get {
                return ResourceManager.GetString("Manifest_InvalidFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The json token {0} is not supported in the manifest file of the module {1}. 的本地化字符串。
        /// </summary>
        internal static string Manifest_InvalidJsonToken {
            get {
                return ResourceManager.GetString("Manifest_InvalidJsonToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The format is invalid at line {0} in the manifest file of the module {1}. 的本地化字符串。
        /// </summary>
        internal static string Manifest_InvalidTextLine {
            get {
                return ResourceManager.GetString("Manifest_InvalidTextLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The manifest does not defined any elements for the module {0}. 的本地化字符串。
        /// </summary>
        internal static string Manifest_NoElement {
            get {
                return ResourceManager.GetString("Manifest_NoElement", resourceCulture);
            }
        }
    }
}
