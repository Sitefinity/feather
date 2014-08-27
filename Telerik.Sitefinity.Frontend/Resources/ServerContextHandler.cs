using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// Instances of this class represent an HTTP handler that returns JavaScript containing dependencies to the server.
    /// </summary>
    public class ServerContextHandler : IHttpHandler
    {
        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
        /// </summary>
        public bool IsReusable
        {
            get 
            {
                return true;
            }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            var script = this.GetScript();

            context.Response.Output.Write(script);
            context.Response.ContentType = "application/x-javascript";
        }

        /// <summary>
        /// Gets the processed script.
        /// </summary>
        /// <remarks>The result will be cached.</remarks>
        /// <returns>The processed script.</returns>
        protected virtual string GetScript()
        {
            if (ServerContextHandler.cachedScript == null)
            {
                lock (ServerContextHandler.scriptLock)
                {
                    if (ServerContextHandler.cachedScript == null)
                    {
                        ServerContextHandler.cachedScript = this.GetRawScript().Replace("{{applicationPath}}", this.GetApplicationPath());
                    }
                }
            }

            return ServerContextHandler.cachedScript;
        }

        /// <summary>
        /// Gets the script from a predefined file.
        /// </summary>
        /// <returns>The script as it is recorded in the file.</returns>
        protected virtual string GetRawScript()
        {
            var jsPath = ServerContextHandler.ScriptPath.Arrange(FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(ServerContextHandler)));
            var serverContextJs = HostingEnvironment.VirtualPathProvider.GetFile(jsPath);

            using (var reader = new StreamReader(serverContextJs.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <returns>The application path.</returns>
        protected virtual string GetApplicationPath()
        {
            return RouteHelper.ResolveUrl("~/", UrlResolveOptions.Rooted);
        }

        private static object scriptLock = new Object();
        private static string cachedScript;
        private const string ScriptPath = "~/{0}Resources/ServerContext.js";
    }
}
