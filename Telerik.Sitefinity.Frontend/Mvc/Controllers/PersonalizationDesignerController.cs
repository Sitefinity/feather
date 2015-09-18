using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Mvc.StringResources;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class contains logic for resolving the views of the MVC designer of a grid.
    /// </summary>
    [Localization(typeof(PersonalizationDesignerResources))]
    [RequestBackendUserAuthentication]
    public class PersonalizationDesignerController : Controller
    {
        /// <summary>
        /// Returns the designer view which handles the property editing for a particular grid. 
        /// If there is custom designer for the particular widget it will be retrieved, otherwise it will fallback to the default designer.
        /// The default designer is located under <see cref="Telerik.Sitefinity.Frontend.Mvc.Views.GridDesigner.Designer.cshtml"/>.
        /// </summary>
        public virtual ActionResult Master()
        {
            this.GetHttpContext().Items[SystemManager.IsBackendRequestKey] = true;

            var title = Res.Get<PersonalizationDesignerResources>().PersonalizationDialogCaption;
            var model = this.GetModel();
            model.Title = title;

            return this.View(PersonalizationDesignerController.DefaultView, model);
        }

        /// <summary>
        /// Gets the HTTP context.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual HttpContextBase GetHttpContext()
        {
            return this.HttpContext;
        }

        /// <summary>
        /// Gets the model of the designer.
        /// </summary>
        private IPersonalizationDesignerModel GetModel()
        {
            return ControllerModelFactory.GetModel<IPersonalizationDesignerModel>(typeof(PersonalizationDesignerController));
        }

        private const string DefaultView = "Personalization";
    }
}
