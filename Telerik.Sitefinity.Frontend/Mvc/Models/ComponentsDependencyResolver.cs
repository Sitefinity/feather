using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Utilities.HtmlParsing;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class handles frontend components by mapping them to scripts and extracting them from markup.
    /// </summary>
    internal static class ComponentsDependencyResolver
    {
        /// <summary>
        /// Gets the scripts.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <returns></returns>
        public static IList<string> GetScripts(IEnumerable<string> components, IEnumerable<string> scripts)
        {
            if (components == null)
            {
                components = new List<string>();
            }

            if (scripts == null)
            {
                scripts = new List<string>();
            }

            var originalScripts = scripts.ToList();

            if (!components.Any())
            {
                return originalScripts;
            }

            var dependencyScripts = new List<string>();

            foreach (var comp in components)
            {
                dependencyScripts.AddRange(ComponentsDependencyResolver.GetComponentScripts(comp));
            }

            return ComponentsDependencyResolver.OrderScripts(dependencyScripts, originalScripts);
        }

        /// <summary>
        /// Extracts the components.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "reader"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "line"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "availableComponents"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "components"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fileStream")]
        public static IList<string> ExtractComponents(Stream fileStream)
        {
            return ComponentsDependencyResolver.ComponentsDefinitionsDictionary.Value.Keys.ToList();
            var components = new List<string>();
            var availableComponents = new HashSet<string>(ComponentsDependencyResolver.ComponentsDefinitionsDictionary.Value.Keys);

            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                }
            }

            return null;
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
            var allScripts = new List<string>();

            if (!string.IsNullOrEmpty(component) && ComponentsDependencyResolver.ComponentsDefinitionsDictionary.Value.ContainsKey(component))
            {
                var componentDefinitionObject = ComponentsDependencyResolver.ComponentsDefinitionsDictionary.Value[component];

                if (componentDefinitionObject.Scripts != null)
                {
                    allScripts.AddRange(componentDefinitionObject.Scripts);
                }

                if (componentDefinitionObject.Components != null)
                {
                    foreach (var comp in componentDefinitionObject.Components)
                    {
                        allScripts.AddRange(ComponentsDependencyResolver.GetComponentScripts(comp));
                    }
                }
            }
            else
            {
                Log.Write(string.Format(System.Globalization.CultureInfo.InvariantCulture, "The component {0} could not be resolved", component));
            }

            return allScripts.Distinct();
        }

        private static Dictionary<string, ScriptDependencyConfigModel> Initialize()
        {
            var assemblyPath = FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(ComponentsDependencyResolver).Assembly);
            var filename = "client-components/components-definitions.json";

            var fileStream = VirtualPathManager.OpenFile("~/" + assemblyPath + filename);

            using (var streamReader = new StreamReader(fileStream))
            {
                var text = streamReader.ReadToEnd();
                return (new JavaScriptSerializer()).Deserialize<Dictionary<string, ScriptDependencyConfigModel>>(text);
            }
        }

        private static readonly Lazy<Dictionary<string, ScriptDependencyConfigModel>> ComponentsDefinitionsDictionary =
           new Lazy<Dictionary<string, ScriptDependencyConfigModel>>(ComponentsDependencyResolver.Initialize, true);
    }
}
