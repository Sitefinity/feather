using System.Collections.Generic;
using System.Linq;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class is used as a model for the designer controller.
    /// </summary>
    public class DesignerModel : IDesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignerModel"/> class.
        /// </summary>
        /// <param name="views">The views that are available to the controller.</param>
        public DesignerModel(IEnumerable<string> views)
        {
            this.views = views.Where(this.IsDesignerView).Select(this.ExtractViewName);
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> Views
        {
            get 
            {
                return this.views;
            }
        }

        /// <summary>
        /// Determines whether the given file represents a designer view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected virtual bool IsDesignerView(string filename)
        {
            return filename.StartsWith("DesignerView.");
        }

        /// <summary>
        /// Extracts the name of the view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected virtual string ExtractViewName(string filename)
        {
            var parts = filename.Split('.');
            if (parts.Length > 2)
                return string.Join(".", parts.Skip(1).Take(parts.Length - 2));
            else if (parts.Length == 2)
                return string.Join(".", parts.Skip(1));
            else
                return filename;
        }

        private IEnumerable<string> views;
    }
}
