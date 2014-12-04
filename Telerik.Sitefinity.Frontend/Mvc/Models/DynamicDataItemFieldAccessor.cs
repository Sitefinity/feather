﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Model;

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
        /// <param name="item">The data item.</param>
        public DynamicDataItemFieldAccessor(IDataItem item)
            : base()
        {
            this.originalItem = item;
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

            string name = binder.Name;
            
            bool isFound;
            var propInfo = TypeDescriptor.GetProperties(this.originalItem)[name];
            if (propInfo != null)
            {
                result = propInfo.GetValue(this.originalItem);
                isFound = true;
            }
            else
            {
                result = null;
                isFound = false;
            }

            return isFound;
        }

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the member.</returns>
        public object GetMemberValue(string fieldName)
        {
            var propInfo = TypeDescriptor.GetProperties(this.originalItem)[fieldName];
            if (propInfo != null)
            {
                return propInfo.GetValue(this.originalItem);
            }
            else
            {
                return null;
            }
        }

        private IDataItem originalItem;
    }
}
