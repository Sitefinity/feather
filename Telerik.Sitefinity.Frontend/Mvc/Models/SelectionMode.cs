namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// The rendering options for a widget. 
    /// </summary>
    /// <remarks>
    /// Each option describes different selection of items that will be included while rendering a widget.
    /// </remarks>
    public enum SelectionMode
    {
        /// <summary>
        /// Refers to all items.
        /// </summary>
        AllItems,

        /// <summary>
        /// Refers to custom selection of items.
        /// </summary>
        SelectedItems,

        /// <summary>
        /// Refers to filtered items based on a custom criteria.
        /// </summary>
        FilteredItems
    }
}
