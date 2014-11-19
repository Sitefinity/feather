namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// The rendering options for the lists.
    /// </summary>
    /// <remarks>
    /// Each option describes different options for dividing items in the list.
    /// </remarks>
    public enum ListDisplayMode
    {
        /// <summary>
        /// Display list divided on pages.
        /// </summary>
        Paging,

        /// <summary>
        /// Display only limited number of items.
        /// </summary>
        Limit,

        /// <summary>
        /// Display all items.
        /// </summary>
        All
    }
}
