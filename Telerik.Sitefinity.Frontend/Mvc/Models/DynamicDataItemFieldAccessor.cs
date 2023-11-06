﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Dynamic;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Utilities.HtmlParsing;
using Telerik.Sitefinity.Web.UI.Fields.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// Instances of this class are used to access fields of data items dynamically.
    /// </summary>
    public class DynamicDataItemFieldAccessor : DynamicObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataItemFieldAccessor"/> class.
        /// </summary>
        /// <param name="itemViewModel">The data item.</param>
        public DynamicDataItemFieldAccessor(ItemViewModel itemViewModel)
            : base()
        {
            this.item = itemViewModel;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");

            var name = binder.Name;
            result = this.GetMemberValue(name);

            return TypeDescriptor.GetProperties(this.item.DataItem)[name] != null;
        }

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the member.</returns>
        public object GetMemberValue(string fieldName)
        {
            var propInfo = TypeDescriptor.GetProperties(this.item.DataItem)[fieldName];
            if (propInfo != null)
            {
                var relatedDataInfo = propInfo as RelatedDataPropertyDescriptor;
                if (relatedDataInfo == null)
                {
                    var value = propInfo.GetValue(this.item.DataItem);
                    var fieldType = propInfo.GetFieldType();
                    if (fieldType != null)
                    {
                        if(fieldType == UserFriendlyDataType.LongText)
                        {
                            var stringValue = this.GetAppropriateStringValue(value);
                            return HtmlFilterProvider.ApplyFilters(stringValue);
                        }
                        else if(fieldType == UserFriendlyDataType.Link)
                        {
                            var stringValue = this.GetAppropriateStringValue(value);
                            return this.ResolveLinkFieldLinks(stringValue);
                        }
                    }

                    return value;
                }
                else
                {
                    if (relatedDataInfo.MetaField.AllowMultipleRelations)
                    {
                        return this.item.RelatedItems(fieldName);
                    }
                    else
                    {
                        return this.item.RelatedItem(fieldName);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private string GetAppropriateStringValue(object value)
        {
            string stringValue = value as Lstring;
            if (stringValue != null)
            {
                return stringValue;
            }

            return value as string;
        }

        private string ResolveLinkFieldLinks(string stringValue)
        {
            var json = JsonConvert.DeserializeObject<LinkItemModel[]>(stringValue);
            foreach (var linkModel in json)
            {
                if (!string.IsNullOrEmpty(linkModel.Sfref))
                {
                    string hrefAnchor = $"<a href=\"{linkModel.Sfref}\"></a>";
                    string resolved = new DynamicLinksParser().Apply(hrefAnchor);

                    using (HtmlParser parser = new HtmlParser(resolved))
                    {
                        parser.SetChunkHashMode(false);
                        parser.AutoExtractBetweenTagsOnly = false;
                        parser.KeepRawHTML = true;
                        var chunk = parser.ParseNext();
                        linkModel.Href = chunk.GetParamValue("href");
                        linkModel.Sfref = chunk.GetParamValue("sfref");
                    }
                }
            }

            return JsonConvert.SerializeObject(json);
        }

        private ItemViewModel item;
    }
}
