using System;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Personalization.Impl.Web;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Personalization
{
    /// <summary>
    /// Defines the common methods for building pages with personalized MVC widgets.
    /// </summary>
    /// <seealso cref="Telerik.Sitefinity.Personalization.Impl.Web.PersonalizedWidgetResolver" />
    public class PersonalizedMvcWidgetResolver : PersonalizedWidgetResolver
    {
        /// <inheritdoc />
        public override Type ResolveWrapperType(Type widgetType)
        {
            if (typeof(MvcProxyBase).IsAssignableFrom(widgetType))
            {
                return typeof(PersonalizedWidgetProxy);
            }
            else
            {
                return base.ResolveWrapperType(widgetType);
            }
        }

        /// <inheritdoc />
        public override void AppendPersonalizationProperties(StringBuilder output, ControlData controlData, Type controlType, Guid pageDataId, out bool appendAllProperties)
        {
            if (typeof(MvcProxyBase).IsAssignableFrom(controlType))
            {
                appendAllProperties = true;

                var controllerName = controlData.Properties.Where(p => p.Name == "ControllerName" && p.Value != null)
                    .Select(p => p.Value).FirstOrDefault();

                if (!controllerName.IsNullOrEmpty())
                {
                    controlType = TypeResolutionService.ResolveType(controllerName, throwOnError: false);
                }

                output.Append(string.Format(" ControlDataId=\"{0}\" ControlTypeName=\"{1}\" PageDataId=\"{2}\"", controlData.Id, controlType.FullName, pageDataId));
            }
            else
            {
                base.AppendPersonalizationProperties(output, controlData, controlType, pageDataId, out appendAllProperties);
            }
        }
    }
}