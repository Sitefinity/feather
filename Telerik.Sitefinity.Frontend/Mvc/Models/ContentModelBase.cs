using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ServiceStack.Text;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// A base class of models that contain logic for querying content.
    /// </summary>
    public abstract class ContentModelBase : IContentLocatableView
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of content that is loaded.
        /// </summary>
        public virtual Type ContentType { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapping element of the widget when it is in List view.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public virtual string ListCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapper div of the widget when it is in Details view.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public virtual string DetailCssClass { get; set; }

        /// <summary>
        /// Gets the list of items to be displayed inside the widget when option "Selected items" is enabled.
        /// </summary>
        /// <value>
        /// The selected item ids.
        /// </value>
        public virtual string SerializedSelectedItemsIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable social sharing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should enable social sharing; otherwise, <c>false</c>.
        /// </value>
        public virtual bool EnableSocialSharing { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets which items to be displayed in the list view.
        /// </summary>
        /// <value>The page display mode.</value>
        public virtual SelectionMode SelectionMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to divide items in the list.
        /// </summary>
        /// <value>
        /// The display mode.
        /// </value>
        public virtual ListDisplayMode DisplayMode { get; set; }

        /// <summary>
        /// Gets or sets the items count per page.
        /// </summary>
        /// <value>
        /// The items per page.
        /// </value>
        public virtual int? ItemsPerPage
        {
            get
            {
                return this.itemsPerPage;
            }

            set
            {
                this.itemsPerPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort expression.
        /// </summary>
        /// <value>
        /// The sort expression.
        /// </value>
        public virtual string SortExpression
        {
            get
            {
                return this.sortExpression;
            }

            set
            {
                this.sortExpression = value;
            }
        }

        /// <summary>
        /// Gets or sets the additional filter expression.
        /// </summary>
        /// <value>
        /// The filter expression.
        /// </value>
        public virtual string FilterExpression { get; set; }

        /// <summary>
        /// Gets or sets the serialized additional filters.
        /// </summary>
        /// <value>
        /// The serialized additional filters.
        /// </value>
        public virtual string SerializedAdditionalFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the canonical URL tag should be added to the page when the canonical meta tag should be added to the page.
        /// If the value is not set, the settings from SystemConfig -> ContentLocationsSettings -> DisableCanonicalURLs will be used. 
        /// </summary>
        /// <value>The disable canonical URLs.</value>
        public virtual bool? DisableCanonicalUrlMetaTag { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the information for all of the content types that a control is able to show.
        /// </summary>
        public virtual IEnumerable<IContentLocationInfo> GetLocations()
        {
            var location = new ContentLocationInfo();
            location.ContentType = this.ContentType;
            location.ProviderName = ManagerBase.GetMappedManager(this.ContentType, this.ProviderName).Provider.Name;

            var filterExpression = this.CompileFilterExpression();
            if (!string.IsNullOrEmpty(filterExpression))
            {
                location.Filters.Add(new BasicContentLocationFilter(filterExpression));
            }

            return new[] { location };
        }

        /// <summary>
        /// Creates a view model for use in list views.
        /// </summary>
        /// <param name="taxonFilter">The taxon filter.</param>
        /// <param name="page">The page.</param>
        /// <returns>A view model for use in list views.</returns>
        /// <exception cref="System.ArgumentException">'page' argument has to be at least 1.;page</exception>
        public virtual ContentListViewModel CreateListViewModel(ITaxon taxonFilter, int page)
        {
            if (page < 1)
                throw new ArgumentException("'page' argument has to be at least 1.", "page");

            var query = this.GetItemsQuery();
            if (query == null)
                return this.CreateListViewModelInstance();

            if (taxonFilter != null)
            {
                var taxonField = this.ExpectedTaxonFieldName(taxonFilter);
                var filter = string.Format(CultureInfo.InvariantCulture, "{0}.Contains({{{1}}})", taxonField, taxonFilter.Id);
                query = query.Where(filter);
            }

            var viewModel = this.CreateListViewModelInstance();
            this.PopulateListViewModel(page, query, viewModel);

            return viewModel;
        }

        /// <summary>
        /// Creates the details view model.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A view model for use in detail views.</returns>
        public virtual ContentDetailsViewModel CreateDetailsViewModel(IDataItem item)
        {
            var viewModel = this.CreateDetailsViewModelInstance();

            viewModel.CssClass = this.DetailCssClass;
            viewModel.Item = item;
            viewModel.ContentType = this.ContentType;
            viewModel.ProviderName = this.ProviderName;
            viewModel.EnableSocialSharing = this.EnableSocialSharing;

            return viewModel;
        }

        /// <summary>
        /// Gets a collection of <see cref="CacheDependencyNotifiedObject"/>.
        ///     The <see cref="CacheDependencyNotifiedObject"/> represents a key for which cached items could be subscribed for
        ///     notification.
        ///     When notified, all cached objects with dependency on the provided keys will expire.
        /// </summary>
        /// <param name="viewModel">View model that will be used for displaying the data.</param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public virtual IList<CacheDependencyKey> GetKeysOfDependentObjects(ContentListViewModel viewModel)
        {
            if (this.ContentType != null)
            {
                var contentResolvedType = this.ContentType;
                var result = new List<CacheDependencyKey>(1);
                result.Add(new CacheDependencyKey { Key = null, Type = contentResolvedType });

                return result;
            }
            else
            {
                return new List<CacheDependencyKey>(0);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="CacheDependencyNotifiedObject"/>.
        ///     The <see cref="CacheDependencyNotifiedObject"/> represents a key for which cached items could be subscribed for
        ///     notification.
        ///     When notified, all cached objects with dependency on the provided keys will expire.
        /// </summary>
        /// <param name="viewModel">View model that will be used for displaying the data.</param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public virtual IList<CacheDependencyKey> GetKeysOfDependentObjects(ContentDetailsViewModel viewModel)
        {
            if (this.ContentType != null)
            {
                var contentResolvedType = this.ContentType;
                var result = new List<CacheDependencyKey>(1);
                if (viewModel.Item != null && viewModel.Item.Id != Guid.Empty)
                {
                    result.Add(new CacheDependencyKey { Key = viewModel.Item.Id.ToString(), Type = contentResolvedType });
                }

                return result;
            }
            else
            {
                return new List<CacheDependencyKey>(0);
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Gets an active query for all items.
        /// </summary>
        /// <returns>The query.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected abstract IQueryable<IDataItem> GetItemsQuery();

        /// <summary>
        /// Applies the list settings.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="query">The items query.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        protected virtual IEnumerable<dynamic> ApplyListSettings(int page, IQueryable<IDataItem> query, out int? totalPages)
        {
            if (page < 1)
                throw new ArgumentException("'page' argument has to be at least 1.", "page");

            int? itemsToSkip = (page - 1) * this.ItemsPerPage;
            itemsToSkip = this.DisplayMode == ListDisplayMode.Paging ? ((page - 1) * this.ItemsPerPage) : null;
            int? totalCount = 0;
            int? take = this.DisplayMode == ListDisplayMode.All ? null : this.ItemsPerPage;

            var compiledFilterExpression = this.CompileFilterExpression();
            compiledFilterExpression = this.AddLiveFilterExpression(compiledFilterExpression);
            compiledFilterExpression = this.AdaptMultilingualFilterExpression(compiledFilterExpression);

            var result = DataProviderBase.SetExpressions(
                query,
                compiledFilterExpression,
                this.SortExpression,
                itemsToSkip,
                take,
                ref totalCount).ToArray<dynamic>();

            totalPages = (int)Math.Ceiling(totalCount.Value / (double)this.ItemsPerPage.Value);
            totalPages = this.DisplayMode == ListDisplayMode.Paging ? totalPages : null;

            return result;
        }

        /// <summary>
        /// Creates a blank instance of a list view model.
        /// </summary>
        /// <returns>The list view model.</returns>
        protected virtual ContentListViewModel CreateListViewModelInstance()
        {
            return new ContentListViewModel();
        }

        /// <summary>
        /// Creates a blank instance of a details view model.
        /// </summary>
        /// <returns>The details view model.</returns>
        protected virtual ContentDetailsViewModel CreateDetailsViewModelInstance()
        {
            return new ContentDetailsViewModel();
        }

        /// <summary>
        /// Compiles a filter expression based on the widget settings.
        /// </summary>
        /// <returns>Filter expression that will be applied on the query.</returns>
        protected virtual string CompileFilterExpression()
        {
            var elements = new List<string>();

            if (this.SelectionMode == SelectionMode.FilteredItems)
            {
                if (!this.SerializedAdditionalFilters.IsNullOrEmpty())
                {
                    var additionalFilters = JsonSerializer.DeserializeFromString<QueryData>(this.SerializedAdditionalFilters);
                    var queryExpression = Telerik.Sitefinity.Data.QueryBuilder.LinqTranslator.ToDynamicLinq(additionalFilters);
                    elements.Add(queryExpression);
                }
            }
            else if (this.SelectionMode == SelectionMode.SelectedItems)
            {
                var selectedItemsFilterExpression = this.GetSelectedItemsFilterExpression();
                if (!selectedItemsFilterExpression.IsNullOrEmpty())
                {
                    elements.Add(selectedItemsFilterExpression);
                }
            }

            if (!this.FilterExpression.IsNullOrEmpty())
            {
                elements.Add(this.FilterExpression);
            }

            return string.Join(" AND ", elements.Select(el => "(" + el + ")"));
        }

        /// <summary>
        /// Populates the list ViewModel.
        /// </summary>
        /// <param name="page">The current page.</param>
        /// <param name="query">The query.</param>
        /// <param name="viewModel">The view model.</param>
        protected virtual void PopulateListViewModel(int page, IQueryable<IDataItem> query, ContentListViewModel viewModel)
        {
            viewModel.CurrentPage = page;

            int? totalPages;
            viewModel.Items = this.ApplyListSettings(page, query, out totalPages);
            viewModel.TotalPagesCount = totalPages;
            viewModel.ProviderName = this.ProviderName;
            viewModel.ContentType = this.ContentType;
            viewModel.CssClass = this.ListCssClass;
            viewModel.ShowPager = this.DisplayMode == ListDisplayMode.Paging && totalPages.HasValue && totalPages > 1;
        }

        #endregion

        #region Private methods

        private string ExpectedTaxonFieldName(ITaxon taxon)
        {
            if (taxon.Taxonomy.Name == "Categories")
                return taxon.Taxonomy.TaxonName;

            return taxon.Taxonomy.Name;
        }

        private string AddLiveFilterExpression(string filterExpression)
        {
            if (filterExpression.IsNullOrEmpty())
            {
                filterExpression = "Visible = true AND Status = Live";
            }
            else
            {
                filterExpression = filterExpression + " AND Visible = true AND Status = Live";
            }

            return filterExpression;
        }

        /// <summary>
        /// Adapts a filter expression in multilingual.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <returns>Multilingual filter expression.</returns>
        private string AdaptMultilingualFilterExpression(string filterExpression)
        {
            CultureInfo uiCulture;
            if (SystemManager.CurrentContext.AppSettings.Multilingual)
            {
                uiCulture = System.Globalization.CultureInfo.CurrentUICulture;
            }
            else
            {
                uiCulture = null;
            }

            // the filter is adapted to the implementation of ILifecycleDataItemGeneric, so the culture is taken in advance when filtering published items.
            return ContentHelper.AdaptMultilingualFilterExpressionRaw(filterExpression, uiCulture);
        }

        private string GetSelectedItemsFilterExpression()
        {
            if (!this.SerializedSelectedItemsIds.IsNullOrEmpty())
            {
                var selectedItemIds = JsonSerializer.DeserializeFromString<IList<string>>(this.SerializedSelectedItemsIds);

                var selectedItemsFilterExpression = string.Join(" OR ", selectedItemIds.Select(id => "Id = " + id.Trim()));
                return selectedItemsFilterExpression;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Privte fields and constants

        private int? itemsPerPage = 20;
        private string sortExpression = "PublicationDate DESC";

        #endregion
    }
}
