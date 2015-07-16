using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class is an HttpHandler that is used for delivering files from a virtual path.
    /// </summary>
    internal class ResourceHttpHandler : IHttpHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceHttpHandler"/> class.
        /// </summary>
        public ResourceHttpHandler(string path)
        {
            this.rootPath = path;
            this.parser = new ResourceTemplateProcessor();
        }

        #region IHttpHandler

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        /// <exception cref="System.Web.HttpException">404;Not found</exception>
        public void ProcessRequest(HttpContext context)
        {
            if (this.FileExists(context.Request.Url.AbsolutePath))
            {
                var fileName = VirtualPathUtility.GetFileName(context.Request.Url.AbsolutePath);

                this.SetResponseClientCache(context, fileName);

                if (!(fileName.EndsWith(".sf-cshtml", StringComparison.OrdinalIgnoreCase) && this.IsWhitelisted(context.Request.Url.AbsolutePath)))
                {
                    using (var fileStream = this.OpenFile(context.Request.Url.AbsolutePath))
                    {
                        this.SendStaticResource(context, fileStream, fileName);
                    }
                }
                else
                {
                    using (new CultureRegion(context.Request.Headers["SF_UI_CULTURE"]))
                    {
                        this.SendParsedTemplate(context);
                    }
                }
            }
            else
            {
                throw new HttpException(404, "Not found");
            }
        }

        #endregion

        /// <summary>
        /// Checks if a files exists on the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        protected virtual bool FileExists(string path)
        {
            return HostingEnvironment.VirtualPathProvider.FileExists(path);
        }

        /// <summary>
        /// Opens the file on the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The stream with the file content.</returns>
        protected virtual Stream OpenFile(string path)
        {
            var file = HostingEnvironment.VirtualPathProvider.GetFile(path);
            return file.Open();
        }

        /// <summary>
        /// Writes the given buffer to output.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        protected virtual void WriteToOutput(HttpContext context, byte[] buffer)
        {
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Determines whether the specified path is whitelisted for executing server code.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Whether the specified path is whitelisted for execution server code.</returns>
        protected virtual bool IsWhitelisted(string path)
        {
            var resolvedRoot = RouteHelper.ResolveUrl("~/" + this.rootPath, UrlResolveOptions.Rooted | UrlResolveOptions.AppendTrailingSlash);
            
            return path.StartsWith(resolvedRoot + "client-components/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith(resolvedRoot + "Mvc/Scripts/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Writes the contents of a static resource to the response.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">Name of the file.</param>
        protected virtual void SendStaticResource(HttpContext context, Stream fileStream, string fileName)
        {
            var buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            context.Response.ContentType = ResourceHttpHandler.GetMimeMapping(fileName);

            this.WriteToOutput(context, buffer);
        }

        /// <summary>
        /// Sends a parsed template that is processed with the Razor engine.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void SendParsedTemplate(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            var packagesManager = new PackageManager();
            var packageName = packagesManager.GetCurrentPackage();
            var templatePath = context.Request.Url.AbsolutePath;

            if (!string.IsNullOrEmpty(packageName))
            {
                templatePath += "#" + packageName;
            }

            var output = this.parser.Process(templatePath);

            this.WriteToOutput(context, context.Response.ContentEncoding.GetBytes(output));
        }

        /// <summary>
        /// Sets client cache for the requested resource. In debug mode this caching is disabled.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="fileName">Name of the file.</param>
        protected virtual void SetResponseClientCache(HttpContext context, string fileName)
        {
#if !DEBUG
            if (fileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".sf-cshtml", StringComparison.OrdinalIgnoreCase))
            {
                var cache = context.Response.Cache;
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetExpires(DateTime.Now + TimeSpan.FromDays(7));
                cache.SetValidUntilExpires(true);
                var lastWriteTime = ResourceHttpHandler.GetAssemblyLastWriteTime();
                cache.SetLastModified(lastWriteTime);
            }
#endif
        }

        /// <summary>
        /// Gets the mime type of the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        private static string GetMimeMapping(string filename)
        {
            var mimeMappingType = Assembly.GetAssembly(typeof(HttpRuntime)).GetType("System.Web.MimeMapping");
            var getMimeMappingMethodInfo = mimeMappingType.GetMethod("GetMimeMapping", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return (string)getMimeMappingMethodInfo.Invoke(null, new object[] { filename });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static DateTime GetAssemblyLastWriteTime()
        {
            var assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            return File.GetLastWriteTime((new Uri(name.CodeBase)).LocalPath);
        }

        private readonly ResourceTemplateProcessor parser;
        private readonly string rootPath;
    }
}
