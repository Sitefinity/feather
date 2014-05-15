using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class is an HttpHandler that is used for delivering files from a virtual path.
    /// </summary>
    public class ResourceHttpHandler : IHttpHandler
    {
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
                using (var fileStream = this.OpenFile(context.Request.Url.AbsolutePath))
                {
                    var fileName = VirtualPathUtility.GetFileName(context.Request.Url.AbsolutePath);
                    var buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, (int)fileStream.Length);

                    this.WriteToOutput(context, buffer);
                    context.Response.ContentType = ResourceHttpHandler.GetMimeMapping(fileName);
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
    }
}
