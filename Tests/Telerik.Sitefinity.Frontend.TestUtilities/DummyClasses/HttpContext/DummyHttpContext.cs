using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext
{
    /// <summary>
    /// This class represents fake <see cref="HttpContextBase"/> that is used for unit testing.
    /// </summary>
    public class DummyHttpContext : HttpContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyHttpContext"/> class.
        /// </summary>
        public DummyHttpContext()
        {
            this.request = new DummyHttpRequest(this, "/");
            this.response = new DummyHttpResponse();
            this.items = new Dictionary<object, object>();
        }

        /// <summary>
        /// When overridden in a derived class, gets the <see cref="T:System.Web.HttpRequest" /> object for the current HTTP request.
        /// </summary>
        /// <returns>The current HTTP request.</returns>
        public override HttpRequestBase Request
        {
            get
            {
                return this.request;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the <see cref="T:System.Web.HttpResponse" /> object for the current HTTP response.
        /// </summary>
        /// <returns>The current HTTP response.</returns>
        public override HttpResponseBase Response
        {
            get
            {
                return this.response;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a key/value collection that can be used to organize and share data between a module and a handler during an HTTP request.
        /// </summary>
        /// <returns>A key/value collection that provides access to an individual value in the collection by using a specified key.</returns>
        public override IDictionary Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns an object for the current service type.
        /// </summary>
        /// <param name="serviceType">The type of service object to get.</param>
        /// <returns>
        /// The current service type, or null if no service is found.
        /// </returns>
        public override object GetService(Type serviceType)
        {
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, gets a IHttpHandler as null object (just to supress NotImplementedException).
        /// </summary>
        /// <returns>Null.</returns>
        public override IHttpHandler CurrentHandler
        {
            get
            {
                return null;
            }
        }

        private HttpRequestBase request;
        private HttpResponseBase response;
        private IDictionary items;
    }
}
