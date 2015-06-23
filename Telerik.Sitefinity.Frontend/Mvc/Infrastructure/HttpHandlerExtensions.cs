using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure
{
    /// <summary>
    /// Holds all extension methods for transforming an <see cref="IHttpHandler"/> to <see cref="Page"/>
    /// </summary>
    public static class HttpHandlerExtensions
    {
        /// <summary>
        /// Casts the IHttpHandler to Page. If the handler is a wrapper of this page - extracts it.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>
        /// The Page or null if the cast failed.
        /// </returns>
        public static Page GetPageHandler(this IHttpHandler handler)
        {
            Page page = null;

            if (handler != null)
            {
                page = handler as Page;

                if (page == null)
                {
                    var pageHandlerWrapperType = Type.GetType("Telerik.Sitefinity.Web.PageHandlerWrapper, Telerik.Sitefinity");
                    if (handler.GetType() == pageHandlerWrapperType)
                    {
                        var baseHandlerField = pageHandlerWrapperType.GetField("baseHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var valueObject = baseHandlerField.GetValue(handler);

                        page = valueObject as Page;
                    }
                }
            }

            return page;
        }
    }
}
