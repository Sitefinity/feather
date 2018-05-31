using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Configuration;
using Telerik.Sitefinity.Taxonomies.Extensions;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class represents the base controller of content widgets.
    /// </summary>
    public abstract class ContentBaseController : Controller
    {
        #region Properties

        /// <summary>
        /// Gets the metadata container.
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
                    this.metadata.OpenGraphType = OpenGraphTypes.Website;
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
            string parentMainValue = string.Empty;
            var seoAndOpenGraphConfig = this.GetSeoAndOpenGraphConfig();
            bool isSeoEnabled = seoAndOpenGraphConfig.EnabledSEO;
            bool isOpenGraphEnabled = seoAndOpenGraphConfig.EnabledOpenGraph;

            if ((!isSeoEnabled && !isOpenGraphEnabled) || !this.IsURLMatch(item))
            {
                return;
            }

            MetadataModel metadataProperties = new MetadataModel();

            if (isSeoEnabled)
            {
                metadataProperties.MetaTitle = this.GetTitleProperty(item, new[] { this.MetadataFields.MetaTitle, MetaDataProperties.MetaTitle });
                metadataProperties.MetaDescription = this.GetDescriptionProperty(item, new[] { this.MetadataFields.MetaDescription, MetaDataProperties.MetaDescription });
            }

            if (isOpenGraphEnabled)
            {
                metadataProperties.OpenGraphTitle = this.GetTitleProperty(item, new[] { this.MetadataFields.OpenGraphTitle, MetaDataProperties.OpenGraphTitle });
                metadataProperties.OpenGraphDescription = this.GetDescriptionProperty(item, new[] { this.MetadataFields.OpenGraphDescription, MetaDataProperties.OpenGraphDescription });
                metadataProperties.Url = this.GetDefaultCanonicalUrl(item);
                metadataProperties.OpenGraphType = this.MetadataFields.OpenGraphType;
                metadataProperties.OpenGraphImage = this.GetFieldValue(item, new[] { this.MetadataFields.OpenGraphImage, MetaDataProperties.OpenGraphImage });
                metadataProperties.OpenGraphVideo = this.GetFieldValue(item, new[] { this.MetadataFields.OpenGraphVideo, MetaDataProperties.OpenGraphVideo });
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
        /// Distinguish the currently opened content item and other content items on the same page that are in details mode.
        /// The current navigated URL is a canonical URL for an item. We take its meta properties.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        internal virtual bool IsURLMatch(IDataItem item)
        {
            IManager manager = null;
            if (!ManagerBase.TryGetMappedManager(item.GetType(), string.Empty, out manager))
                return true;

            var locationsService = SystemManager.GetContentLocationService();
            var locations = locationsService.GetItemLocations(item);
            var currentNode = (PageSiteNode)SystemManager.CurrentHttpContext.Items[SiteMapBase.CurrentNodeKey];
            var itemUrl = HyperLinkHelpers.GetDetailPageUrl(item, currentNode.Id);

            // TODO: compare urls better
            return locations.Any(location => location.ItemAbsoluteUrl == itemUrl);
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

            return null;
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

            return this.GetFieldValue(itemValue, field);
        }

        private string GetMetaValue(object detailItem, string fieldName)
        {
            var property = TypeDescriptor.GetProperties(detailItem)[fieldName];

            if (property == null)
            {
                return null;
            }

            var taxonProperty = property as TaxonomyPropertyDescriptor;
            if (taxonProperty != null)
            {
                return this.GetTextFromTaxa(detailItem, taxonProperty);
            }

            var relatedProperty = property as RelatedDataPropertyDescriptor;
            if (relatedProperty != null)
            {
                var mediaItem = detailItem as IDataItem;
                var mediaItemViewModel = new ItemViewModel(mediaItem);
                ItemViewModel relatedItem = null;

                if (relatedProperty.MetaField.AllowMultipleRelations)
                {
                    relatedItem = mediaItemViewModel.RelatedItems(fieldName).FirstOrDefault();
                }
                else
                {
                    relatedItem = mediaItemViewModel.RelatedItem(fieldName);
                }

                if (relatedItem != null)
                {
                    var relatedMediaItem = relatedItem.DataItem as MediaContent;
                    if (relatedMediaItem != null)
                    {
                        var relatedItemMediaUrl = relatedMediaItem.MediaUrl;
                        if (relatedItemMediaUrl != null || !string.IsNullOrEmpty(relatedItemMediaUrl))
                        {
                            return relatedItemMediaUrl;
                        }
                    }
                }

                return null;
            }

            var value = property.GetValue(detailItem);
            if (value == null || value.ToString().IsNullOrEmpty())
            {
                return null;
            }

            return value.ToString();
        }

        private string GetTextFromTaxa(object item, TaxonomyPropertyDescriptor descriptor)
        {
            var taxaText = descriptor.GetTaxaText(item);

            if (!string.IsNullOrEmpty(taxaText))
            {
                return taxaText;
            }

            return null;
        }

        private string GetTitleProperty(object detailItem, string[] propertyNames)
        {
            var fields = new List<string>(propertyNames);

            if (typeof(IDynamicContentWidget).IsAssignableFrom(this.GetType()))
            {
                var dynamicType = this.GetDynamicContentType();
                var title = dynamicType == null ? MetaDataProperties.Title : dynamicType.MainShortTextFieldName;
                fields.Add(title);
            }
            else
            {
                fields.Add(MetaDataProperties.Title);
            }

            return this.GetFieldValue(detailItem, fields.ToArray());
        }

        private string GetDescriptionProperty(object detailItem, string[] propertyNames)
        {
            var fields = new List<string>(propertyNames);
            fields.Add(MetaDataProperties.Description);

            return this.GetFieldValue(detailItem, fields.ToArray());
        }

        private string GetFieldValue(object item, string[] propertyNames)
        {
            var i = 0;
            foreach (var propertyName in propertyNames)
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var value = this.GetMetaValue(item, propertyName);
                    // The first property is the main property.
                    // If the user has specified such property and the property does not exist, 
                    // the metadata should not be added.
                    if (i == 0) { return value; }

                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }

                i++;
            }

            return null;
        }

        #endregion

        #region Fields and constants

        protected struct OpenGraphTypes
        {
            public const string Website = "website";
            public const string Article = "article";
            public const string Video = "video";
            public const string Image = "image";
        }

        private static class MetaDataProperties
        {
            internal const string MetaTitle = "MetaTitle";
            internal const string MetaDescription = "MetaDescription";
            internal const string OpenGraphTitle = "OpenGraphTitle";
            internal const string OpenGraphDescription = "OpenGraphDescription";
            internal const string OpenGraphImage = "OpenGraphImage";
            internal const string OpenGraphVideo = "OpenGraphVideo";
            internal const string Title = "Title";
            internal const string Description = "Description";
        }

        private MetadataModel metadata;

        #endregion
    }
}
