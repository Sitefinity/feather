using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    internal class GridDesignerModel : IGridDesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDesignerModel"/> class.
        /// </summary>
        /// <param name="gridTitle">Title of the grid.</param>
        /// <param name="controlId">The control identifier.</param>
        public GridDesignerModel(string gridTitle, string controlId)
        {
            this.GridTitle = gridTitle;
            this.ControlId = controlId;
        }

        /// <summary>
        /// Gets or sets the grid title.
        /// </summary>
        /// <value>
        /// The grid title.
        /// </value>
        public string GridTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the control identifier.
        /// </summary>
        /// <value>
        /// The control identifier.
        /// </value>
        public string ControlId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the grid update service URL.
        /// </summary>
        /// <value>
        /// The grid update service URL.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public string GridUpdateServiceUrl
        {
            get 
            {
                return RouteHelper.ResolveUrl(GridDesignerModel.GridUpdateUrlFormat, UrlResolveOptions.Rooted);
            }
        }

        private const string GridUpdateUrlFormat = "/Sitefinity/Services/Pages/ZoneEditorService.svc/Layout/Style/{0}/{1}/{2}/";
    }
}
