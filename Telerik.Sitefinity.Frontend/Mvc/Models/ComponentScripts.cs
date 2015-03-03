using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    public static class ComponentScripts
    {
        public static IEnumerable<string> Get(string component)
        {
            if (!string.IsNullOrEmpty(component) && ComponentsScripts.ContainsKey(component))
            {
                return ComponentsScripts[component];
            }

            return null;
        }

        private static readonly Dictionary<string, string[]> ComponentsScripts = new Dictionary<string, string[]>()
        {
            { 
                "ChangePassword", new string[] 
                { 
                    "Mvc/Scripts/Angular/angular-resource.min.js",
	                "Mvc/Scripts/expander.js",
	                "Mvc/Scripts/style-dropdown.js",
	                "client-components/selectors/common/sf-services.js",
	                "client-components/selectors/common/sf-selectors.js",
	                "client-components/selectors/common/sf-items-tree.js",
	                "client-components/selectors/common/sf-list-selector.js",
	                "client-components/selectors/pages/sf-page-service.js",
	                "client-components/selectors/pages/sf-page-selector.js" 
                } 
            },
        };
    }
}
