using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    public abstract class ContentControllerBase<TModel> : Controller
        where TModel : ContentModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the template that will be displayed when widget is in List view.
        /// </summary>
        public virtual string ListTemplateName
        {
            get
            {
                return this.listTemplateName;
            }

            set
            {
                this.listTemplateName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the template that will be displayed when widget is in Detail view.
        /// </summary>
        public virtual string DetailTemplateName
        {
            get
            {
                return this.detailTemplateName;
            }

            set
            {
                this.detailTemplateName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the canonical URL tag should be added to the page when the canonical meta tag should be added to the page.
        /// If the value is not set, the settings from SystemConfig -> ContentLocationsSettings -> DisableCanonicalURLs will be used. 
        /// </summary>
        /// <value>The disable canonical URLs.</value>
        [Browsable(false)]
        public virtual bool? DisableCanonicalUrlMetaTag
        {
            get
            {
                return this.Model.DisableCanonicalUrlMetaTag;
            }

            set
            {
                this.Model.DisableCanonicalUrlMetaTag = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether detail view for a news item should be opened in the same page.
        /// </summary>
        /// <value>
        /// <c>true</c> if details link should be opened in the same page; otherwise, (if should redirect to custom selected page)<c>false</c>.
        /// </value>
        public virtual bool OpenInSamePage
        {
            get
            {
                return this.openInSamePage;
            }

            set
            {
                this.openInSamePage = value;
            }
        }

        /// <summary>
        /// Gets or sets the page URL where will be displayed details view for selected news item.
        /// </summary>
        /// <value>
        /// The page URL where will be displayed details view for selected news item.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string DetailsPageUrl
        {
            get
            {
                if (this.OpenInSamePage)
                {
                    var url = this.GetCurrentPageUrl();
                    return url;
                }
                else
                {
                    return this.detailsPageUrl;
                }
            }

            set
            {
                this.detailsPageUrl = value;
            }
        }

        /// <summary>
        /// Gets the News widget model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TModel Model
        {
            get
            {
                if (this.model == null)
                    this.model = this.InitializeModel();

                return this.model;
            }
        }

        /// <summary>
        /// Gets the type of the content that is relevant for this controller.
        /// </summary>
        /// <returns></returns>
        protected abstract Type ContentType { get; }

        #endregion

        #region Actions

        /// <summary>
        /// Renders appropriate list view depending on the <see cref="ListTemplateName" />
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public virtual ActionResult Index(int? page)
        {
            var fullTemplateName = this.listTemplateNamePrefix + this.ListTemplateName;
            this.ViewBag.RedirectPageUrlTemplate = "/{0}";
            this.ViewBag.DetailsPageUrl = this.DetailsPageUrl;

            this.Model.PopulateItems(null, null, page);
            this.AddCacheDependencies();

            return this.View(fullTemplateName, this.Model);
        }

        /// <summary>
        /// Renders appropriate list view depending on the <see cref="ListTemplateName" />
        /// </summary>
        /// <param name="taxonFilter">The taxonomy filter.</param>
        /// <param name="page">The page.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ListByTaxon(ITaxon taxonFilter, int? page)
        {
            var fullTemplateName = this.listTemplateNamePrefix + this.ListTemplateName;
            var fieldName = this.GetExpectedTaxonFieldName(taxonFilter);
            this.ViewBag.RedirectPageUrlTemplate = "/" + taxonFilter.UrlName + "/{0}";
            this.ViewBag.DetailsPageUrl = this.DetailsPageUrl;

            this.Model.PopulateItems(taxonFilter, fieldName, page);
            this.AddCacheDependencies();

            return this.View(fullTemplateName, this.Model);
        }

        /// <summary>
        /// Gets the information for all of the content types that a control is able to show.
        /// </summary>
        /// <returns>
        /// List of location info of the content that this control is able to show.
        /// </returns>
        [NonAction]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<IContentLocationInfo> GetLocations()
        {
            return this.Model.GetLocations();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            this.Model.ContentType = this.ContentType;
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            this.AddCacheDependencies();    
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <returns>
        /// The <see cref="IDynamicContentModel"/>.
        /// </returns>
        private TModel InitializeModel()
        {
            return ControllerModelFactory.GetModel<TModel>(this.GetType());
        }

        /// <summary>
        /// Adds the cache dependencies.
        /// </summary>
        private void AddCacheDependencies()
        {
            if (SystemManager.CurrentHttpContext != null)
            {
                this.AddCacheDependencies(this.Model.GetKeysOfDependentObjects());
            }
        }

        private string GetExpectedTaxonFieldName(ITaxon taxon)
        {
            if (taxon.Taxonomy.Name == "Categories")
                return taxon.Taxonomy.TaxonName;

            return taxon.Taxonomy.Name;
        }

        #endregion

        #region Private fields and constants

        private TModel model;
        private string listTemplateName = "DynamicContentList";
        private string listTemplateNamePrefix = "List.";
        private string detailTemplateName = "DetailPage";
        private bool openInSamePage = true;
        private string detailsPageUrl;

        #endregion
    }
}
