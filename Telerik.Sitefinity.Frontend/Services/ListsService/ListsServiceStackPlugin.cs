using System;
using System.Linq;
using ServiceStack;
using ServiceStack.Text;
using Telerik.Sitefinity.Frontend.Services.ListsService.DTO;
using Telerik.Sitefinity.Modules.Lists.Web.Services.Data;

namespace Telerik.Sitefinity.Frontend.Services.ListsService
{
    /// <summary>
    /// Represents a ServiceStack plug-in for the Lists web service.
    /// </summary>
    internal class ListsServiceStackPlugin : IPlugin
    {
        /// <summary>
        /// Adding the lists service routes
        /// </summary>
        /// <param name="appHost">The service stack appHost</param>
        public void Register(IAppHost appHost)
        {
            if (appHost == null)
                throw new ArgumentNullException("appHost");

            //// NOTE: exclude this property as it causes cycle loop reference exception when serializing.
            JsConfig<ListViewModel>.ExcludePropertyNames = new[] { "AvailableCultures", "ContentItem" };

            appHost.RegisterService<ListsWebService>();
            appHost.Routes.Add<SpecificListsGetRequest>(string.Concat(ListServiceWebUrl, "/", "items"), ApplyTo.Put);
        }

        private const string ListServiceWebUrl = "/lists-api";
    }
}
