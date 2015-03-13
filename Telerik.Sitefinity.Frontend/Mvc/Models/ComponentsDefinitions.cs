using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class resolves script dependencies for components.
    /// </summary>
    public static class ComponentsDefinitions
    {
        private static readonly Lazy<Dictionary<string, ComponentsDefinitionsConfigModel>> ComponentsDefinitionsDictionary =
            new Lazy<Dictionary<string, ComponentsDefinitionsConfigModel>>(ComponentsDefinitions.Initialize, true);

        /// <summary>
        /// Gets the scripts.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <returns></returns>
        public static IList<string> GetScripts(IEnumerable<string> components, IEnumerable<string> scripts)
        {
            var originalScripts = scripts.ToList();

            if (components == null || !components.Any())
            {
                return originalScripts;
            }

            var dependencyScripts = new List<string>();

            foreach (var comp in components)
            {
                dependencyScripts.AddRange(ComponentsDefinitions.GetComponentScripts(comp));
            }

            return ComponentsDefinitions.OrderScripts(dependencyScripts, originalScripts);
        }

        private static IList<string> OrderScripts(IList<string> dependencyScripts, IEnumerable<string> originalScripts)
        {
            var scripts = dependencyScripts.Concat(originalScripts).Distinct().ToList();

            if (scripts.Any(x => x.Contains("sf-list-selector")))
            {
                scripts.Insert(0, "client-components/selectors/common/sf-list-selector.js");
            }

            scripts.Insert(0, "client-components/selectors/common/sf-selectors.js");
            scripts.Insert(0, "client-components/selectors/common/sf-services.js");

            if (scripts.Any(x => x.Contains("sf-fields")))
            {
                scripts.Insert(0, "client-components/fields/sf-fields.js");
            }

            var mvcScripts = scripts.Where(x => x.IndexOf("Mvc/Scripts/", 0, StringComparison.Ordinal) >= 0).ToList();
            foreach (var mvcScript in mvcScripts)
            {
                scripts.Insert(0, mvcScript);
            }

            var angularScripts = scripts.Where(x => x.IndexOf("angular", 0, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            foreach (var angularScript in angularScripts)
            {
                scripts.Insert(0, angularScript);
            }

            return scripts.Distinct().ToList();
        }

        private static IEnumerable<string> GetComponentScripts(string component)
        {
            if (!string.IsNullOrEmpty(component) && ComponentsDefinitions.ComponentsDefinitionsDictionary.Value.ContainsKey(component))
            {
                var allScripts = new List<string>();

                var componentDefinitionObject = ComponentsDefinitions.ComponentsDefinitionsDictionary.Value[component];

                if (componentDefinitionObject.Scripts != null)
                {
                    allScripts.AddRange(componentDefinitionObject.Scripts);
                }

                if (componentDefinitionObject.Components != null)
                {
                    foreach (var comp in componentDefinitionObject.Components)
                    {
                        allScripts.AddRange(ComponentsDefinitions.GetComponentScripts(comp));
                    }
                }

                return allScripts.Distinct();
            }

            return null;
        }

        private static Dictionary<string, ComponentsDefinitionsConfigModel> Initialize()
        {
            var assemblyPath = FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(ComponentsDefinitions).Assembly);
            var filename = "client-components/components-definitions.json";

            var fileStream = VirtualPathManager.OpenFile("~/" + assemblyPath + filename);

            using (var streamReader = new StreamReader(fileStream))
            {
                var text = streamReader.ReadToEnd();
                return (new JavaScriptSerializer()).Deserialize<Dictionary<string, ComponentsDefinitionsConfigModel>>(text);
            }
        }
    }
}
