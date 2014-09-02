using System;

namespace Telerik.Sitefinity.Frontend.Designers
{
    /// <summary>
    /// This attribute can be used to specify the URL of the MVC designer for the particular widget.
    /// </summary>
    /// <remarks> 
    /// Widget designers are meant to provide simple and straightforward user interface for setting widget properties.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DesignerUrlAttribute : Attribute
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignerUrlAttribute"/> class.
        /// </summary>
        /// <param name="url">The designer URL.</param>
        public DesignerUrlAttribute(string url)
        {
            this.url = url;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the URL of the widget designer.
        /// </summary>
        /// <value>The URL of the widget designer.</value>
        public virtual string Url
        {
            get
            {
                return this.url;
            }
        }     

        #endregion

        #region Private fields

        private string url;

        #endregion
    }
}
