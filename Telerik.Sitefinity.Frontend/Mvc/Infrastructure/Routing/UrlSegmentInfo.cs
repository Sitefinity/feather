using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// This class represents information about single segment from the URL template.
    /// </summary>
    internal class UrlSegmentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlSegmentInfo"/> class.
        /// </summary>
        public UrlSegmentInfo() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlSegmentInfo"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="resolverName">Name of the resolver.</param>
        public UrlSegmentInfo(string parameterName, string resolverName, object parameterValue)
        {
            this.ParameterName = parameterName;
            this.ResolverName = resolverName;
            this.ParameterValue = parameterValue;
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the name of the resolver.
        /// </summary>
        /// <value>
        /// The name of the resolver.
        /// </value>
        public string ResolverName { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        public object ParameterValue { get; set; }
    }
}
