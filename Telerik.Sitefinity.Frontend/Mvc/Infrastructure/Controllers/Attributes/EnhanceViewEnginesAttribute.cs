using System;
using System.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// This attribute defines how a view engine paths are enhanced by the controller factory for the marked <see cref="Controller"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EnhanceViewEnginesAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether view engines should be enhanced.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disabled; otherwise, <c>false</c>.
        /// </value>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the virtual path that contains the Views folder with the controller views.
        /// </summary>
        public string VirtualPath 
        { 
            get
            {
                return this.virtualPath;
            }

            set
            {
                this.virtualPath = VirtualPathUtility.AppendTrailingSlash(value);
            }
        }

        private string virtualPath;
    }
}
