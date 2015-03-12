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
        /// <param name="component">The component.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetScripts(string component)
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
                        allScripts.AddRange(ComponentsDefinitions.GetScripts(comp));
                    }
                }

                return allScripts.Distinct();
            }

            return null;
        }

        /// <summary>
        /// Gets the scripts.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetScripts(IEnumerable<string> components)
        {
            var allScripts = new List<string>();

            if (components != null)
            {
                foreach (var comp in components)
                {
                    allScripts.AddRange(ComponentsDefinitions.GetScripts(comp));
                }
            }

            return allScripts.Distinct();
        }

        private static Dictionary<string, ComponentsDefinitionsConfigModel> Initialize()
        {
            var assemblyPath = FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(ComponentsDefinitions).Assembly);
            var filename = "client-components/components-definitions.json";

            var fileStream = VirtualPathManager.OpenFile(assemblyPath + filename);

            using (var streamReader = new StreamReader(fileStream))
            {
                var text = streamReader.ReadToEnd();
                return (new JavaScriptSerializer()).Deserialize<Dictionary<string, ComponentsDefinitionsConfigModel>>(text);
            }
        }
    }
}
