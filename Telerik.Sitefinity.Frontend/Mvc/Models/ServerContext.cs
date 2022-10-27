using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Licensing;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.BlobStorage;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ServerContext
    {
        public string ApplicationPath
        {
            get { return RouteHelper.ResolveUrl("~/", UrlResolveOptions.Rooted); }
        }

        public string CurrentPackage
        {
            get { return new PackageManager().GetCurrentPackage() ?? string.Empty; }
        }

        public string FrontendLanguages
        {
            get
            {
                var appSettings = SystemManager.CurrentContext.AppSettings;
                var languages = appSettings.DefinedFrontendLanguages.Select(l => l.Name);

                var serialziedLanguages = JsonSerializer.SerializeToString(languages);

                return serialziedLanguages;
            }
        }

        public string CurrentFrontendRootNodeId
        {
            get { return SiteInitializer.CurrentFrontendRootNodeId.ToString(); }
        }

        public string CurrentUserId
        {
            get
            {
                var identity = ClaimsManager.GetCurrentIdentity();
                var id = identity == null ? Guid.Empty : identity.UserId;

                return id.ToString();
            }
        }

        public string CurrentSiteId
        {
            get { return SystemManager.CurrentContext.CurrentSite.Id.ToString(); }
        }

        public string IsMultisiteMode
        {
            get { return true.ToString(); }
        }

        public string[] DamSupportedMediaTypes 
        {
            get
            {
                IEnumerable<string> damSupportedMediaTypes = Enumerable.Empty<string>();
                if (LicenseState.CheckIsModuleLicensed(LibrariesModule.DamIntegrationModuleId))
                {
                    var provider = BlobStorageManager.GetManager().StaticProviders.OfType<DamBlobStorageProviderBase>().FirstOrDefault();
                    if (provider != null)
                    {
                        damSupportedMediaTypes = provider.GetSupportedMediaTypes().Select(x => x.FullName);
                    }
                }

                return damSupportedMediaTypes.ToArray();
            }
        }
    }
}
