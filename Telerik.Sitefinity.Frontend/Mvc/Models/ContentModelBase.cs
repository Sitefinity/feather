using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ServiceStack.Text;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    public abstract class ContentModelBase : ICacheDependable, IContentLocatableView
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of content that is loaded.
        /// </summary>
        public virtual Type ContentType { get; set; }

        /// <summary>
        /// Gets an enumerable of the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public virtual IEnumerable<dynamic> Items
        {
            get
            {
                return this.items;
            }

            private set
            {
                this.items = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapping element of the widget when it is in List view.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public virtual string ListCssClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapper div of the widget when it is in Details view.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public virtual string DetailCssClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Id of the item that should be displayed when filtering by preselected items.
        /// </summary>
        /// <value>
        /// The selected items.
        /// </value>
        public virtual Guid SelectedItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the detail item.
        /// </summary>
        /// <value>
        /// The detail news.
        /// </value>
        public virtual dynamic DetailItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable social sharing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should enable social sharing; otherwise, <c>false</c>.
        /// </value>
        public virtual bool EnableSocialSharing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public virtual string ProviderName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets which items to be displayed in the list view.
        /// </summary>
        /// <value>The page display mode.</value>
        public virtual SelectionMode SelectionMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to divide items in the list.
        /// </summary>
        /// <value>
        /// The display mode.
        /// </value>
        public virtual ListDisplayMode DisplayMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total pages count.
        /// </summary>
        /// <value>
        /// The total pages count.
        /// </value>
        public virtual int? TotalPagesCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public virtual int CurrentPage
        {
            get;
            set;
        }

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
        public virtual string FilterExpression
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the query data used for filtering of the news items.
        /// </summary>
        /// <value>
        /// The additional filters.
        /// </value>
        public virtual QueryData AdditionalFilters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the serialized additional filters.
        /// </summary>
        /// <value>
        /// The serialized additional filters.
        /// </value>
        public virtual string SerializedAdditionalFilters
        {
            get
            {
                return this.serializedAdditionalFilters;
            }

            set
            {
                if (this.serializedAdditionalFilters != value)
                {
                    this.serializedAdditionalFilters = value;
                    if (!this.serializedAdditionalFilters.IsNullOrEmpty())
                    {
                        this.AdditionalFilters = JsonSerializer.DeserializeFromString<QueryData>(this.serializedAdditionalFilters);
                    }
                }
            }
        }

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
        /// Populates the items.
        /// </summary>
        /// <param name="taxonFilter">The taxon that should be contained in the items.</param>
        /// <param name="taxonField">The taxon field.</param>
        /// <param name="page">The page.</param>
        public abstract void PopulateItems(ITaxon taxonFilter, string taxonField, int? page);

        /// <summary>
        /// Gets a collection of <see cref="CacheDependencyNotifiedObject"/>.
        ///     The <see cref="CacheDependencyNotifiedObject"/> represents a key for which cached items could be subscribed for
        ///     notification.
        ///     When notified, all cached objects with dependency on the provided keys will expire.
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public virtual IList<CacheDependencyKey> GetKeysOfDependentObjects()
        {
            if (this.ContentType != null)
            {
                var contentResolvedType = this.ContentType;
                var result = new List<CacheDependencyKey>(1);
                if (this.DetailItem != null && this.DetailItem.Id != Guid.Empty)
                {
                    result.Add(new CacheDependencyKey { Key = this.DetailItem.Id.ToString(), Type = contentResolvedType });
                }
                else
                {
                    result.Add(new CacheDependencyKey { Key = null, Type = contentResolvedType });
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
        /// Applies the list settings.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="query">The items query.</param>
        protected virtual void ApplyListSettings(int? page, IQueryable<IDataItem> query)
        {
            if (page == null || page < 1)
                page = 1;

            int? itemsToSkip = (page.Value - 1) * this.ItemsPerPage;
            itemsToSkip = this.DisplayMode == ListDisplayMode.Paging ? ((page.Value - 1) * this.ItemsPerPage) : null;
            int? totalCount = 0;
            int? take = this.DisplayMode == ListDisplayMode.All ? null : this.ItemsPerPage;

            var compiledFilterExpression = this.CompileFilterExpression();
            compiledFilterExpression = this.AddLiveFilterExpression(compiledFilterExpression);
            compiledFilterExpression = this.AdaptMultilingualFilterExpression(compiledFilterExpression);

            this.Items = DataProviderBase.SetExpressions(
                query,
                compiledFilterExpression,
                this.SortExpression,
                itemsToSkip,
                take,
                ref totalCount).ToArray<dynamic>();

            this.TotalPagesCount = (int)Math.Ceiling((double)(totalCount.Value / (double)this.ItemsPerPage.Value));
            this.TotalPagesCount = this.DisplayMode == ListDisplayMode.Paging ? this.TotalPagesCount : null;
            this.CurrentPage = page.Value;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Compiles a filter expression based on the widget settings.
        /// </summary>
        /// <returns>Filter expression that will be applied on the query.</returns>
        private string CompileFilterExpression()
        {
            var elements = new List<string>();

            if (this.SelectionMode == SelectionMode.FilteredItems)
            {
                if (this.AdditionalFilters != null)
                {
                    var queryExpression = Telerik.Sitefinity.Data.QueryBuilder.LinqTranslator.ToDynamicLinq(this.AdditionalFilters);
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
            var selectedItemIds = new List<Guid>() { this.SelectedItemId };

            var selectedItemsFilterExpression = string.Join(" OR ", selectedItemIds.Select(id => "Id = " + id));
            return selectedItemsFilterExpression;
        }

        #endregion

        #region Privte fields and constants

        private IEnumerable<dynamic> items;
        private int? itemsPerPage = 20;
        private string sortExpression = "PublicationDate DESC";
        private string serializedAdditionalFilters;

        #endregion
    }
}
