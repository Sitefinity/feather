using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext
{
    /// <summary>
    /// This class represents fake <see cref="HttpRequestBase"/> for unit testing.
    /// </summary>
    public class DummyHttpRequest : HttpRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyHttpRequest"/> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="appPath">The application path.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public DummyHttpRequest(HttpContextBase httpContext, string appPath)
        {
            if (appPath == null)
                throw new ArgumentNullException("appPath");

            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            this.applicationPath = appPath;
            this.requestContext = new RequestContext(httpContext, new RouteData());
            this.serverVariables = new NameValueCollection();
        }

        /// <summary>
        /// When overridden in a derived class, gets the virtual root path of the ASP.NET application on the server.
        /// </summary>
        /// <returns>The virtual path of the current application.</returns>
        public override string ApplicationPath
        {
            get
            {
                return this.applicationPath;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the <see cref="T:System.Web.Routing.RequestContext" /> instance of the current request.
        /// </summary>
        /// <returns>The <see cref="T:System.Web.Routing.RequestContext" /> instance of the current request. For non-routed requests, the <see cref="T:System.Web.Routing.RequestContext" /> object that is returned is empty.</returns>
        public override RequestContext RequestContext
        {
            get
            {
                return this.requestContext;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the collection of HTTP query-string variables.
        /// </summary>
        /// <returns>The Query string.</returns>
        public override NameValueCollection QueryString
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a collection of Web server variables.
        /// </summary>
        /// <returns>The server variables.</returns>
        public override NameValueCollection ServerVariables
        {
            get
            {
                return this.serverVariables;
            }
        }

        private string applicationPath;
        private RequestContext requestContext;
        private NameValueCollection serverVariables;
    }
}
