using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Configuration;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.ContentLocations;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class represents the base controller of content widgets.
    /// </summary>
    public abstract class ContentBaseController : Controller
    {
        #region Properties

        /// <summary>
        /// Gets the metadata container.Title for search engines
        /// 
        /// </summary>
        /// <value>
        /// The metadata container.
        /// </value>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual MetadataModel MetadataFields
        {
            get
            {
                if (this.metadata == null)
                {
                    this.metadata = new MetadataModel();
                    this.metadata.OpenGraphType = PageHelper.OpenGraphTypes.Website;
                    this.metadata.PageTitleMode = Telerik.Sitefinity.Mvc.ControllerActionInvoker.PageTitleModes.Replace;
                }

                return this.metadata;
            }
            set
            {
                metadata = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populates the viewbag with values for metadata properties.
        /// </summary>
        /// <param name="item">The item.</param>
        protected void InitializeMetadataDetailsViewBag(IDataItem item)
        {
            if (this.IsDesignMode && !this.IsPreviewMode)
            {
                return;
            }

            string parentMainValue = string.Empty;
            var seoAndOpenGraphConfig = this.GetSeoAndOpenGraphConfig();
            MetadataModel metadataProperties = new MetadataModel();

            metadataProperties.SEOEnabled = seoAndOpenGraphConfig.EnabledSEO;
            metadataProperties.OpenGraphEnabled = seoAndOpenGraphConfig.EnabledOpenGraph;
            metadataProperties.SEOEnabledPerWidget = this.MetadataFields.SEOEnabled;
            metadataProperties.OpenGraphEnabledPerWidget = this.MetadataFields.OpenGraphEnabled;
            bool isSeoEnabled = metadataProperties.SEOEnabled && metadataProperties.SEOEnabledPerWidget;
            bool isOpenGraphEnabled = metadataProperties.OpenGraphEnabled && metadataProperties.OpenGraphEnabledPerWidget;      

            if (isSeoEnabled)
            {
                metadataProperties.MetaTitle = this.GetTitleProperty(item, new[] { this.MetadataFields.MetaTitle, PageHelper.MetaDataProperties.MetaTitle });
                metadataProperties.MetaDescription = this.GetDescriptionProperty(item, new[] { this.MetadataFields.MetaDescription, PageHelper.MetaDataProperties.MetaDescription });
            }

            if (isOpenGraphEnabled)
            {
                metadataProperties.OpenGraphTitle = this.GetTitleProperty(item, new[] { this.MetadataFields.OpenGraphTitle, PageHelper.MetaDataProperties.OpenGraphTitle, this.MetadataFields.MetaTitle, PageHelper.MetaDataProperties.MetaTitle });
                metadataProperties.OpenGraphDescription = this.GetDescriptionProperty(item, new[] { this.MetadataFields.OpenGraphDescription, PageHelper.MetaDataProperties.OpenGraphDescription, this.MetadataFields.MetaDescription, PageHelper.MetaDataProperties.MetaDescription });
                metadataProperties.Url = this.GetDefaultCanonicalUrl(item);
                metadataProperties.OpenGraphType = this.MetadataFields.OpenGraphType;
                metadataProperties.OpenGraphImage = PageHelper.GetFieldValue(item, new[] { this.MetadataFields.OpenGraphImage, PageHelper.MetaDataProperties.OpenGraphImage });
                metadataProperties.OpenGraphVideo = PageHelper.GetFieldValue(item, new[] { this.MetadataFields.OpenGraphVideo, PageHelper.MetaDataProperties.OpenGraphVideo });
                metadataProperties.SiteName = SystemManager.CurrentContext.CurrentSite.Name;
            }

            if (isSeoEnabled || isOpenGraphEnabled)
            {
                metadataProperties.PageTitleMode = this.MetadataFields.PageTitleMode;

                if (this.MetadataFields.PageTitleMode == Telerik.Sitefinity.Mvc.ControllerActionInvoker.PageTitleModes.Hierarchy && typeof(IDynamicContentWidget).IsAssignableFrom(this.GetType()))
                {
                    parentMainValue = this.GetParentMainFiledValue(item);
                    if (!string.IsNullOrEmpty(parentMainValue))
                    {
                        this.ViewBag.ParentMainValue = parentMainValue;
                    }
                }
            }

            this.ViewBag.Metadata = metadataProperties;
        }

        /// <summary>
        /// Determines whether the page is in design mode.
        /// </summary>
        protected virtual bool IsDesignMode
        {
            get
            {
                return SystemManager.IsDesignMode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current request is for page in preview mode.
        /// </summary>
        protected virtual bool IsPreviewMode
        {
            get
            {
                return SystemManager.IsPreviewMode;
            }
        }

        /// <summary>
        /// Gets the item default location.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The item default location.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "The url is needed here as a string.")]
        internal virtual string GetDefaultCanonicalUrl(IDataItem item)
        {
            IManager manager = null;
            if (!ManagerBase.TryGetMappedManager(item.GetType(), string.Empty, out manager))
                return null;

            var locationsService = SystemManager.GetContentLocationService();
            var location = locationsService.GetItemDefaultLocation(item);
            if (location != null)
            {
                return location.ItemAbsoluteUrl;
            }

            var page = this.HttpContext.CurrentHandler.GetPageHandler();
            var pageNode = SiteMapBase.GetActualCurrentNode();
            var canonicalUrl = page.GetCanonicalUrlForPage(pageNode);

            return canonicalUrl;
        }
        
        internal virtual SeoAndOpenGraphElement GetSeoAndOpenGraphConfig()
        {
            return Config.Get<SystemConfig>().SeoAndOpenGraphConfig;
        }
        
        private string GetParentMainFiledValue(IDataItem item)
        {
            string parentMainValue = string.Empty;
            var property = TypeDescriptor.GetProperties(item)["ParentItem"];
            if (property != null)
            {
                var parent = property.GetValue(item);
                parentMainValue = this.GetParentMainValueFromProperty(parent);

                var parentProperty = TypeDescriptor.GetProperties(parent)["ParentItem"];

                if (parentProperty != null)
                {
                    var grandParent = property.GetValue(parent);
                    parentMainValue = parentMainValue + " - " + this.GetParentMainValueFromProperty(grandParent);
                }
            }

            return parentMainValue;
        }

        private string GetParentMainValueFromProperty(object itemValue)
        {
            var parentMainField = this.GetDynamicContentType().ParentModuleType.MainShortTextFieldName;
            string[] field = { parentMainField };

            return PageHelper.GetFieldValue(itemValue, field);
        }

        private string GetTitleProperty(object detailItem, string[] propertyNames)
        {
            var fields = new List<string>(propertyNames);

            if (typeof(IDynamicContentWidget).IsAssignableFrom(this.GetType()))
            {
                var dynamicType = this.GetDynamicContentType();
                var title = dynamicType == null ? PageHelper.MetaDataProperties.Title : dynamicType.MainShortTextFieldName;
                fields.Add(title);
            }
            else
            {
                fields.Add(PageHelper.MetaDataProperties.Title);
            }

            return PageHelper.GetFieldValue(detailItem, fields.ToArray());
        }

        private string GetDescriptionProperty(object detailItem, string[] propertyNames)
        {
            var fields = new List<string>(propertyNames);
            fields.Add(PageHelper.MetaDataProperties.Description);

            return PageHelper.GetFieldValue(detailItem, fields.ToArray());
        }

        #endregion

        #region Fields and constants    

        private MetadataModel metadata;

        #endregion
    }
}