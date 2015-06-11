using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.GeoLocations.Model;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Locations.Configuration;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class represents view model for items.
    /// </summary>
    /// <remarks>
    /// It is used in Master/detail widgets.
    /// </remarks>
    public class ItemViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ItemViewModel(IDataItem item)
        {
            this.DataItem = item;
            this.Fields = new DynamicDataItemFieldAccessor(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the data item that is represented by this view model.
        /// </summary>
        /// <value>
        /// The data item.
        /// </value>
        public IDataItem DataItem { get; private set; }

        /// <summary>
        /// Gets a property that accesses fields of the data item that is represented by this view model.
        /// </summary>
        public dynamic Fields { get; private set; }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <value>
        /// The default URL.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string DefaultUrl
        {
            get
            {
                if (this.DataItem != null)
                    return this.DataItem.GetDefaultUrl();
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the value of the identifier field for the item.
        /// </summary>
        /// <example><see cref="NewsItem"/> identifier is its Title.</example>
        /// <value>
        /// The identifier value.
        /// </value>
        public object Identifier
        {
            get
            {
                if (this.DataItem != null)
                {
                    var field = RelatedDataHelper.GetRelatedTypeIdentifierField(this.DataItem.GetType().FullName);
                    return this.Fields.GetMemberValue(field);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Serializes to JSON.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual string SerializeToJson(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("SerializeToJson", fieldName);

            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as string;

            var fieldValue = this.Fields.GetMemberValue(fieldName);
            var serializedValue = new JavaScriptSerializer().Serialize(fieldValue);
            this.cachedFieldValues[cahcedResultKey] = serializedValue;

            return serializedValue;
        }

        #region Address field

        /// <summary>
        /// Gets the address value string depending on the provided format.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="format">The address format.</param>
        /// <returns></returns>
        public virtual string GetAddressString(string fieldName, string format)
        {
            var cahcedResultKey = this.FieldCacheKey("GetAddressString", fieldName);

            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as string;

            string result = string.Empty;
            var fieldValue = this.Fields.GetMemberValue(fieldName) as Address;

            if (fieldValue != null && !format.IsNullOrEmpty())
            {
                var street = string.Empty;
                if (!string.IsNullOrEmpty(fieldValue.Street))
                {
                    street = fieldValue.Street + ",";
                }

                result = format.Replace("#=Street#", street);

                var zip = string.Empty;
                if (!string.IsNullOrEmpty(fieldValue.Zip))
                {
                    zip = fieldValue.Zip + ",";
                }

                result = result.Replace("#=Zip#", zip);

                var city = string.Empty;
                if (!string.IsNullOrEmpty(fieldValue.City))
                {
                    city = fieldValue.City + ",";
                }

                result = result.Replace("#=City#", city);

                var country = Telerik.Sitefinity.Configuration.Config.Get<LocationsConfig>().Countries[fieldValue.CountryCode];
                string countryName = string.Empty;
                var state = string.Empty;
                if (country != null)
                {
                    countryName = country.Name;
                    if (!fieldValue.StateCode.IsNullOrEmpty() &&
                        (fieldValue.CountryCode == "CA" || fieldValue.CountryCode == "US"))
                    {
                        var stateData = Telerik.Sitefinity.Configuration.Config.Get<LocationsConfig>().Countries[fieldValue.CountryCode]
                            .StatesProvinces[fieldValue.StateCode];
                        if (stateData != null)
                        {
                            state = stateData.Name;
                        }
                    }
                }

                result = result.Replace("#=Country#", countryName);
                result = result.Replace("#=State#", state);
            }

            this.cachedFieldValues[cahcedResultKey] = result;

            return result;
        }

        #endregion

        #region DateTime field

        /// <summary>
        /// Gets formatted date time value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="format">The field format.</param>
        /// <returns></returns>
        public virtual string GetDateTime(string fieldName, string format)
        {
            var dateTimeValue = (DateTime)this.Fields.GetMemberValue(fieldName);

            if (dateTimeValue == default(DateTime))
                return null;

            var formattedDate = dateTimeValue.ToSitefinityUITime().ToString(format);

            return formattedDate;
        }

        #endregion

        #region Price field

        /// <summary>
        /// Gets formatted date time value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="format">The field format.</param>
        /// <returns></returns>
        public virtual string GetPrice(string fieldName, string format)
        {
            var fieldValue = this.Fields.GetMemberValue(fieldName);
            if (fieldValue == null)
                return null;

            var formattedValue = string.Format(fieldValue.ToString(), format);

            return formattedValue;
        }

        #endregion

        #region Yes/No field

        /// <summary>
        /// Gets field value as Yes/No string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual string GetBool(string fieldName)
        {
            var boolValue = (bool)this.Fields.GetMemberValue(fieldName);

            return boolValue ? Res.Get<Labels>().Yes : Res.Get<Labels>().No;
        }

        #endregion

        #region Choice fields

        /// <summary>
        /// Gets the multiple choice value string.
        /// </summary>
        /// <param name="multiChoiceValues">The multi choice values.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "multi")]
        public virtual string GetMultipleChoiceValueString(string fieldName)
        {
            var multiChoiceValues = this.Fields.GetMemberValue(fieldName) as IEnumerable;
            if (multiChoiceValues == null)
                return null;

            StringBuilder sb = new StringBuilder();

            foreach (object val in multiChoiceValues)
            {
                sb.Append(val.ToString());
                sb.Append(", ");
            }

            if (sb.Length > 1)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        #endregion

        #region Taxon fields

        /// <summary>
        /// Gets the flat taxons.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="classificationId">The classification identifier.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Taxons")]
        public virtual IList<FlatTaxon> GetFlatTaxons(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("GetFlatTaxonNames", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as IList<FlatTaxon>;

            var taxonIds = this.Fields.GetMemberValue(fieldName) as IList<Guid>;
            TaxonomyManager manager = TaxonomyManager.GetManager();

            var taxonNames = manager.GetTaxa<FlatTaxon>()
                    .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Name == fieldName).ToList();

            this.cachedFieldValues[cahcedResultKey] = taxonNames;

            return taxonNames;
        }

        /// <summary>
        /// Gets the list of hierarchical taxons.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Taxons")]
        public virtual IList<HierarchicalTaxon> GetHierarchicalTaxons(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("GetHierarchicalTaxonNames", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as IList<HierarchicalTaxon>;

            var taxonIds = this.Fields.GetMemberValue(fieldName) as IList<Guid>;
            string taxonomyName;
            TaxonomyManager manager = TaxonomyManager.GetManager();
            if (fieldName == "Category")
                taxonomyName = "Categories";
            else if (fieldName == "Department")
                taxonomyName = "Departments";
            else
                taxonomyName = fieldName;

            var taxonNames = manager.GetTaxa<HierarchicalTaxon>()
                   .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Name == taxonomyName).ToList();
            this.cachedFieldValues[cahcedResultKey] = taxonNames;

            return taxonNames;
        }

        /// <summary>
        /// Gets the flat taxon.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual FlatTaxon GetFlatTaxon(string fieldName)
        {
            var taxon = this.GetFlatTaxons(fieldName).FirstOrDefault();

            return taxon;
        }

        /// <summary>
        /// Gets the hierarchical taxon.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual HierarchicalTaxon GetHierarchicalTaxon(string fieldName)
        {
            var taxon = this.GetHierarchicalTaxons(fieldName).FirstOrDefault();

            return taxon;
        }

        #endregion

        #region Related data fileds

        /// <summary>
        /// Gets related items.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual IEnumerable<ItemViewModel> RelatedItems(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("RelatedItems", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as IEnumerable<ItemViewModel>;

            IEnumerable<ItemViewModel> result;

            IEnumerable<IDataItem> relatedItems;
            if (!this.TryGetResultFromRelatedDataPropertyDescriptor<IEnumerable<IDataItem>>(fieldName, out relatedItems))
            {
                relatedItems = this.DataItem.GetRelatedItems(fieldName);
            }

            if (relatedItems != null)
                result = relatedItems.ToArray().Select(item => new ItemViewModel(item)).ToArray();
            else
                result = null;

            this.cachedFieldValues[cahcedResultKey] = result;

            return result;
        }

        /// <summary>
        /// Gets the related item when single.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual ItemViewModel RelatedItem(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("RelatedItem", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as ItemViewModel;

            ItemViewModel result;
            IDataItem relatedItem;
            if (this.TryGetResultFromRelatedDataPropertyDescriptor<IDataItem>(fieldName, out relatedItem))
            {
                result = relatedItem == null ? null : new ItemViewModel(relatedItem);
            }
            else
            {
                var relatedItems = this.DataItem.GetRelatedItems(fieldName);

                if (relatedItems != null)
                    result = relatedItems.ToArray().Select(item => new ItemViewModel(item)).FirstOrDefault();
                else
                    result = null;
            }

            this.cachedFieldValues[cahcedResultKey] = result;

            return result;
        }

        private bool TryGetResultFromRelatedDataPropertyDescriptor<T>(string fieldName, out T param)
        {
            param = default(T);
            var propInfo = TypeDescriptor.GetProperties(this.DataItem)[fieldName];
            var relatedDataPropertyDescriptor = propInfo as RelatedDataPropertyDescriptor;
            if (relatedDataPropertyDescriptor != null)
            {
                var result = relatedDataPropertyDescriptor.GetValue(this.DataItem);
                if (result != null && typeof(T).IsAssignableFrom(result.GetType()))
                {
                    param = (T)result;
                }

                return true;
            }

            return false;
        }
        #endregion

        #endregion

        #region Private members

        /// <summary>
        /// Gets the cache key for <see cref="cachedFieldValues"/>.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private string FieldCacheKey(string methodName, string fieldName)
        {
            return methodName + ":" + fieldName;
        }

        /// <summary>
        /// Contains the cached field values.
        /// </summary>
        private Dictionary<string, object> cachedFieldValues = new Dictionary<string, object>();

        #endregion
    }
}
