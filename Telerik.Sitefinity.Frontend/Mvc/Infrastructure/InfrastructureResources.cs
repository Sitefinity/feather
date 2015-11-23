using System.Diagnostics.CodeAnalysis;
using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure
{
    /// <summary>
    /// Localizable strings for the Frontend infrastructure.
    /// </summary>
    [ObjectInfo(typeof(InfrastructureResources), Title = "InfrastructureResourcesTitle", Description = "InfrastructureResourcesDescription")]
    public class InfrastructureResources : Resource
    {
        /// <summary>
        /// Title for the infrastructure resources class.
        /// </summary>
        [ResourceEntry("InfrastructureResourcesTitle",
            Value = "Frontend Infrastructure resources",
            Description = "Title for the infrastructure resources class.",
            LastModified = "2015/01/12")]
        public string InfrastructureResourcesTitle
        {
            get
            {
                return this["InfrastructureResourcesTitle"];
            }
        }

        /// <summary>
        /// Description for the infrastructure resources class.
        /// </summary>
        [ResourceEntry("InfrastructureResourcesDescription",
            Value = "Localizable strings for the Frontend infrastructure.",
            Description = "Description for the infrastructure resources class.",
            LastModified = "2015/01/12")]
        public string InfrastructureResourcesDescription
        {
            get
            {
                return this["InfrastructureResourcesDescription"];
            }
        }

        /// <summary>
        /// Message that is to be displayed for a widget when its controller execution throws exception.
        /// </summary>
        [ResourceEntry("ErrorExecutingController",
            Value = "Exception occured while executing the controller. Check error logs for details.",
            Description = "Description for the infrastructure resources class.",
            LastModified = "2015/01/12")]
        public string ErrorExecutingController
        {
            get
            {
                return this["ErrorExecutingController"];
            }
        }

        /// <summary>
        /// Caption of the Feather config section.
        /// </summary>
        [ResourceEntry("FeatherConfigCaption",
            Value = "Feather",
            Description = "Caption of the Feather config section.",
            LastModified = "2015/11/23")]
        public string FeatherConfigCaption
        {
            get
            {
                return this["FeatherConfigCaption"];
            }
        }

        /// <summary>
        /// Description of the Feather config section.
        /// </summary>
        [ResourceEntry("FeatherConfigDescription",
            Value = "Configuration for the Feather module.",
            Description = "Description of the Feather config section.",
            LastModified = "2015/11/23")]
        public string FeatherConfigDescription
        {
            get
            {
                return this["FeatherConfigDescription"];
            }
        }

        /// <summary>
        /// Caption of the 'Disable precompiled views' checkbox.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Precompilation")]
        [ResourceEntry("DisablePrecompilationCaption",
            Value = "Disable precompiled views",
            Description = "Caption of the 'Disable precompiled views' checkbox.",
            LastModified = "2015/11/23")]
        public string DisablePrecompilationCaption
        {
            get
            {
                return this["DisablePrecompilationCaption"];
            }
        }

        /// <summary>
        /// Description of the 'Disable precompiled views' checkbox.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Precompilation")]
        [ResourceEntry("DisablePrecompilationDescription",
            Value = "Feather will use precompiled view templated unless this checkbox is checked.",
            Description = "Description of the 'Disable precompiled views' checkbox.",
            LastModified = "2015/11/23")]
        public string DisablePrecompilationDescription
        {
            get
            {
                return this["DisablePrecompilationDescription"];
            }
        }

        /// <summary>
        /// Caption of the 'Always use precompiled version' checkbox.
        /// </summary>
        [ResourceEntry("AlwaysUsePrecompiledVersionCaption",
            Value = "Always use precompiled version",
            Description = "Caption of the 'Always use precompiled version' checkbox.",
            LastModified = "2015/11/23")]
        public string AlwaysUsePrecompiledVersionCaption
        {
            get
            {
                return this["AlwaysUsePrecompiledVersionCaption"];
            }
        }

        /// <summary>
        /// Description of the 'Always use precompiled version' checkbox.
        /// </summary>
        [ResourceEntry("AlwaysUsePrecompiledVersionDescription",
            Value = "When checked Feather will always use the precompiled version of a resource regardless of changes to physical files.",
            Description = "Description of the 'Always use precompiled version' checkbox.",
            LastModified = "2015/11/23")]
        public string AlwaysUsePrecompiledVersionDescription
        {
            get
            {
                return this["AlwaysUsePrecompiledVersionDescription"];
            }
        }
    }
}
