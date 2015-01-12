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
    }
}
