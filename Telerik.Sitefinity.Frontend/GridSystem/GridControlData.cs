namespace Telerik.Sitefinity.Frontend.GridSystem
{
    /// <summary>
    /// This class is used as a data model when creating grid controls 
    /// </summary>
    public class GridControlData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
       
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
       
        /// <summary>
        /// Gets or sets the layout template path.
        /// </summary>
        /// <value>
        /// The layout template path.
        /// </value>
        public string LayoutTemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public string CssClass { get; set; }
    }
}
