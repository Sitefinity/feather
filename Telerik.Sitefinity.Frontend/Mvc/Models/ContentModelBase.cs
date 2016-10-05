﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ServiceStack.Text;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.RelatedData;
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
        public virtual string SerializedSelectedItemsIds
        {
            get
            {
                return this.serializedSelectedItemsIds;
            }

            set
            {
                if (this.serializedSelectedItemsIds != value)
                {
                    this.serializedSelectedItemsIds = value;
                    if (!this.serializedSelectedItemsIds.IsNullOrEmpty())
                    {
                        this.selectedItemsIds = JsonSerializer.DeserializeFromString<IList<string>>(this.serializedSelectedItemsIds);
                    }
                }
            }
        }

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
        /// Gets or sets the items limit count.
        /// </summary>
        /// <value>
        /// The items limit.
        /// </value>
        public virtual int? LimitCount
        {
            get
            {
                return this.limitCount;
            }

            set
            {
                this.limitCount = value;
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
        /// Gets or sets the serialized date filters.
        /// </summary>
        /// <value>
        /// The serialized date filters.
        /// </value>
        public virtual string SerializedDateFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the canonical URL tag should be added to the page when the canonical meta tag should be added to the page.
        /// If the value is not set, the settings from SystemConfig -> ContentLocationsSettings -> DisableCanonicalURLs will be used. 
        /// </summary>
        /// <value>The disable canonical URLs.</value>
        public virtual bool? DisableCanonicalUrlMetaTag { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent item.
        /// </summary>
        /// <value>
        /// The type of the parent item.
        /// </value>
        public virtual string RelatedItemType { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent item provider.
        /// </summary>
        /// <value>
        /// The name of the parent item provider.
        /// </value>
        public virtual string RelatedItemProviderName { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public virtual string RelatedFieldName { get; set; }

        /// <summary>
        /// Gets or sets the relation type of the items that will be display - children or parent.
        /// </summary>
        /// <value>
        /// The relation type of the items that will be display - children or parent.
        /// </value>
        public virtual RelationDirection RelationTypeToDisplay { get; set; }

        /// <summary>
        /// Gets or sets the URL key prefix.
        /// </summary>
        /// <value>The URL key prefix.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string UrlKeyPrefix { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the information for all of the content types that a control is able to show.
        /// </summary>
        public virtual IEnumerable<IContentLocationInfo> GetLocations()
        {
            var location = new ContentLocationInfo();
            location.ContentType = this.ContentType;
            location.ProviderName = this.GetManager().Provider.Name;

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

            var viewModel = this.CreateListViewModelInstance();
            viewModel.UrlKeyPrefix = this.UrlKeyPrefix;
            var query = this.GetItemsQuery();
            if (query == null)
                return viewModel;

            if (taxonFilter != null)
            {
                var taxonomy = taxonFilter.Taxonomy.RootTaxonomy ?? taxonFilter.Taxonomy;

                string taxonomyField;

                if (this.TryGetTaxonomyFieldName(taxonomy.Id, out taxonomyField))
                {
                    var filter = string.Format(CultureInfo.InvariantCulture, "{0}.Contains({{{1}}})", taxonomyField, taxonFilter.Id);
                    query = query.Where(filter);
                }
                else
                {
                    viewModel.Items = Enumerable.Empty<ItemViewModel>();
                    this.SetViewModelProperties(viewModel, page, null);
                    return viewModel;
                }
            }

            this.PopulateListViewModel(page, query, viewModel);

            return viewModel;
        }

        /// <summary>
        /// Creates a view model for use in list views filtered by related item.
        /// </summary>
        /// <param name="relatedItem">Item that is related to the resulting items</param>
        /// <param name="page">The page.</param>
        /// <returns>A view model for use in list views.</returns>
        /// <exception cref="System.ArgumentException">'page' argument has to be at least 1.;page</exception>
        public virtual ContentListViewModel CreateListViewModelByRelatedItem(IDataItem relatedItem, int page)
        {
            if (page < 1)
                throw new ArgumentException("'page' argument has to be at least 1.", "page");

            if (relatedItem == null)
                throw new ArgumentNullException("relatedItem");

            int? totalCount = 0;
            var query = this.GetRelatedItems(relatedItem, 1, ref totalCount);

            var viewModel = this.CreateListViewModelInstance();

            viewModel.Items = query.ToArray().Select(item => this.CreateItemViewModelInstance(item)).ToArray();

            if (this.ItemsPerPage != 0)
                viewModel.TotalPagesCount = totalCount / this.ItemsPerPage;

            viewModel.CurrentPage = page;
            viewModel.ProviderName = this.ProviderName;
            viewModel.ContentType = this.ContentType;
            viewModel.CssClass = this.ListCssClass;
            viewModel.ShowPager = this.DisplayMode == ListDisplayMode.Paging && viewModel.TotalPagesCount.HasValue && viewModel.TotalPagesCount > 1;

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
            viewModel.Item = this.CreateItemViewModelInstance(item);
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
                if (viewModel.Item != null && viewModel.Item.Fields.Id != Guid.Empty)
                {
                    result.Add(new CacheDependencyKey { Key = viewModel.Item.Fields.Id.ToString(), Type = contentResolvedType });
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
        protected virtual IEnumerable<ItemViewModel> ApplyListSettings(int page, IQueryable<IDataItem> query, out int? totalPages)
        {
            if (page < 1)
                throw new ArgumentException("'page' argument has to be at least 1.", "page");

            int? itemsToSkip = (page - 1) * this.ItemsPerPage;
            itemsToSkip = this.DisplayMode == ListDisplayMode.Paging ? ((page - 1) * this.ItemsPerPage) : null;
            int? totalCount = 0;
            int? take = null;

            if (this.DisplayMode == ListDisplayMode.Limit)
            {
                take = this.LimitCount;   
            }
            else if (this.DisplayMode == ListDisplayMode.Paging)
            {
                 take = this.ItemsPerPage;   
            }

            IList<ItemViewModel> result = new List<ItemViewModel>();

            query = this.UpdateExpression(query, itemsToSkip, take, ref totalCount);

            var queryResult = this.FetchItems(query);

            foreach (var item in queryResult)
            {
                result.Add(this.CreateItemViewModelInstance(item));
            }

            totalPages = (totalCount.Value + this.itemsPerPage.Value - 1) / this.ItemsPerPage.Value;
            totalPages = this.DisplayMode == ListDisplayMode.Paging ? totalPages : null;

            return result;
        }

        /// <summary>
        /// Fetches the items from a queryable.
        /// </summary>
        /// <param name="query">The queryable.</param>
        /// <returns>Fetched items.</returns>
        protected virtual IEnumerable<IDataItem> FetchItems(IQueryable<IDataItem> query)
        {
            var queryResult = query.ToArray<IDataItem>();
            return queryResult;
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
        /// Creates an instance of the item view model by given data item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected virtual ItemViewModel CreateItemViewModelInstance(IDataItem item)
        {
            return new ItemViewModel(item);
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
                    if (additionalFilters.QueryItems != null && additionalFilters.QueryItems.Length > 0)
                    {
                        var queryExpression = Telerik.Sitefinity.Data.QueryBuilder.LinqTranslator.ToDynamicLinq(additionalFilters);
                        elements.Add(queryExpression);
                    }
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
            int? totalPages = null;
            if (this.SelectionMode == Models.SelectionMode.SelectedItems && this.selectedItemsIds.Count == 0)
            {
                viewModel.Items = Enumerable.Empty<ItemViewModel>();
            }
            else
            {
                viewModel.Items = this.ApplyListSettings(page, query, out totalPages);
            }

            this.SetViewModelProperties(viewModel, page, totalPages);
        }

        /// <summary>
        /// Sets the view model properties.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="page">The page.</param>
        /// <param name="totalPages">The total pages.</param>
        protected virtual void SetViewModelProperties(ContentListViewModel viewModel, int page, int? totalPages)
        {
            viewModel.CurrentPage = page;
            viewModel.TotalPagesCount = totalPages;
            viewModel.ProviderName = this.ProviderName;
            viewModel.ContentType = this.ContentType;
            viewModel.CssClass = this.ListCssClass;
            viewModel.ShowPager = this.DisplayMode == ListDisplayMode.Paging && totalPages.HasValue && totalPages > 1;
        }

        /// <summary>
        /// Gets a manager instance for the model.
        /// </summary>
        /// <returns>The manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ContentType")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual IManager GetManager()
        {
            if (this.ContentType == null)
                throw new InvalidOperationException("Cannot resolve manager because ContentType is not set.");

            return ManagerBase.GetMappedManager(this.ContentType, this.ProviderName);
        }

        /// <summary>
        /// Updates the expression.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#")]
        protected virtual IQueryable<IDataItem> UpdateExpression(IQueryable<IDataItem> query, int? skip, int? take, ref int? totalCount)
        {
            var compiledFilterExpression = this.CompileFilterExpression();
            compiledFilterExpression = this.AddLiveFilterExpression(compiledFilterExpression);
            compiledFilterExpression = this.AdaptMultilingualFilterExpression(compiledFilterExpression);

            query = this.SetExpression(
                query,
                compiledFilterExpression,
                this.SortExpression,
                skip,
                take,
                ref totalCount);

            return query;
        }

        /// <summary>
        /// Appends the lifecycle conditions to the filter expression in order to get only live items.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <returns>Filter expression for live items.</returns>
        protected virtual string AddLiveFilterExpression(string filterExpression)
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
        protected virtual string AdaptMultilingualFilterExpression(string filterExpression)
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

        /// <summary>
        /// Modifies the given query with the given filter, sort expression and paging.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="sortExpr">The sort expression.</param>
        /// <param name="itemsToSkip">The items to skip.</param>
        /// <param name="itemsToTake">The items to take.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>Resulting filtered query.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expr"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#")]
        protected virtual IQueryable<TItem> SetExpression<TItem>(IQueryable<TItem> query, string filterExpression, string sortExpr, int? itemsToSkip, int? itemsToTake, ref int? totalCount)
            where TItem : IDataItem
        {
            if (sortExpr == "AsSetManually")
            {
                query = DataProviderBase.SetExpressions(
                                                  query,
                                                  filterExpression,
                                                  string.Empty,
                                                  null,
                                                  null,
                                                  ref totalCount);

                query = query.OfType<ILifecycleDataItemGeneric>()
                    .Select(x => new
                    {
                        item = x,
                        orderIndex = this.selectedItemsIds.IndexOf(x.OriginalContentId.ToString()) >= 0 ?
                                        this.selectedItemsIds.IndexOf(x.OriginalContentId.ToString()) :
                                        this.selectedItemsIds.IndexOf(x.Id.ToString())
                    })
                    .OrderBy(x => x.orderIndex)
                    .Select(x => x.item)
                    .OfType<TItem>();

                if (itemsToSkip.HasValue && itemsToSkip.Value > 0)
                {
                    query = query.Skip(itemsToSkip.Value);
                }

                if (itemsToTake.HasValue && itemsToTake.Value > 0)
                {
                    query = query.Take(itemsToTake.Value);
                }
            }
            else
            {
                try
                {
                    query = DataProviderBase.SetExpressions(
                                                      query,
                                                      filterExpression,
                                                      sortExpr,
                                                      itemsToSkip,
                                                      itemsToTake,
                                                      ref totalCount);
                }
                catch (MemberAccessException)
                {
                    this.SortExpression = DefaultSortExpression;
                    query = DataProviderBase.SetExpressions(
                                                      query,
                                                      filterExpression,
                                                      this.SortExpression,
                                                      itemsToSkip,
                                                      itemsToTake,
                                                      ref totalCount);
                }
            }

            return query;
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Tries to get the name of the taxonomy field.
        /// </summary>
        /// <param name="taxonomyId">The taxonomy id.</param>
        /// <param name="taxonomyField">The taxonomy field.</param>
        /// <returns></returns>
        private bool TryGetTaxonomyFieldName(Guid taxonomyId, out string taxonomyField)
        {
            taxonomyField = string.Empty;

            var taxonomyPropertyDescriptor = FieldHelper.GetFields(this.ContentType)
                                                        .OfType<TaxonomyPropertyDescriptor>()
                                                        .FirstOrDefault(t => t.TaxonomyId == taxonomyId);

            if (taxonomyPropertyDescriptor != null)
            {
                taxonomyField = taxonomyPropertyDescriptor.Name;
            }

            return taxonomyPropertyDescriptor != null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Telerik.Sitefinity", "SF1002:AvoidToListOnIEnumerable")]
        private string GetSelectedItemsFilterExpression()
        {
            var selectedItemGuids = this.selectedItemsIds.Select(id => new Guid(id));
            var masterIds = this.GetItemsQuery()
                .OfType<ILifecycleDataItemGeneric>()
                .Where(c => selectedItemGuids.Contains(c.Id) || selectedItemGuids.Contains(c.OriginalContentId))
                .Select(n => n.OriginalContentId != Guid.Empty ? n.OriginalContentId : n.Id)
                .Distinct();

            var selectedItemConditions = masterIds.Select(id => "Id = {0} OR OriginalContentId = {0}".Arrange(id.ToString("D")));
            var selectedItemsFilterExpression = string.Join(" OR ", selectedItemConditions);

            return selectedItemsFilterExpression;
        }

        private IQueryable<IDataItem> GetRelatedItems(IDataItem relatedItem, int page, ref int? totalCount)
        {
            var manager = this.GetManager();
            var relatedDataSource = manager.Provider as IRelatedDataSource;

            if (relatedDataSource == null)
                return Enumerable.Empty<IDataItem>().AsQueryable();

            //// Тhe filter is adapted to the implementation of ILifecycleDataItemGeneric, so the culture is taken in advance when filtering published items.
            var filterExpression = ContentHelper.AdaptMultilingualFilterExpression(this.FilterExpression);
            int? skip = (page - 1) * this.ItemsPerPage;

            var relatedItems = relatedDataSource.GetRelatedItems(this.RelatedItemType, this.RelatedItemProviderName, relatedItem.Id, this.RelatedFieldName, this.ContentType, ContentLifecycleStatus.Live, filterExpression, this.SortExpression, skip, this.ItemsPerPage, ref totalCount, this.RelationTypeToDisplay)
                .OfType<IDataItem>();

            return relatedItems;
        }
        #endregion

        #region Private fields and constants

        private const string DefaultSortExpression = "PublicationDate DESC";

        private int? itemsPerPage = 20;
        private int? limitCount = 20;
        private string sortExpression = DefaultSortExpression;
        private string serializedSelectedItemsIds;
        private IList<string> selectedItemsIds = new List<string>();

        #endregion
    }
}
