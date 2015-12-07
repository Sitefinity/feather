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
        /// <param name="scripts">The scripts.</param>
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
        /// <param name="fileStream">The file stream.</param>
        /// <returns></returns>
        public static IList<string> ExtractComponents(Stream fileStream)
        {
            var candidateComponents = new HashSet<string>();
            using (var reader = new StreamReader(fileStream))
            {
                using (HtmlParser parser = new HtmlParser(reader.ReadToEnd()))
                {
                    HtmlChunk chunk = null;
                    parser.SetChunkHashMode(false);
                    parser.AutoExtractBetweenTagsOnly = false;
                    parser.CompressWhiteSpaceBeforeTag = false;
                    parser.KeepRawHTML = false;
                    parser.AutoKeepComments = false;

                    while ((chunk = parser.ParseNext()) != null)
                    {
                        if (chunk.Type == HtmlChunkType.OpenTag)
                        {
                            //// Angular directives can be tag name (E)
                            candidateComponents.Add(chunk.TagName.ToLower());
                            for (int i = 0; i < chunk.Attributes.Length; i++)
                            {
                                //// The html parser has no more attributes
                                if (chunk.Values[i] == null)
                                    break;

                                //// Angular directives can be class attribute value (C)
                                if (chunk.Attributes[i].ToLower() == "class")
                                    candidateComponents.Add(chunk.Values[i].ToLower());
                                else
                                    //// Angular directives can be attribute name (A)
                                    candidateComponents.Add(chunk.Attributes[i].ToLower());
                            }
                        }
                    }
                }
            }

            candidateComponents.IntersectWith(ComponentsDependencyResolver.AvailableComponents.Value.Keys);

            return candidateComponents.Select(key => ComponentsDependencyResolver.AvailableComponents.Value[key]).ToList();
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

        private static Dictionary<string, ScriptDependencyConfigModel> InitializeComponentsDefinitions()
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

        private static Dictionary<string, string> InitializeAvailableComponents()
        {
            var availableComponents = new Dictionary<string, string>();

            var knownComponentsNamingMismatches = new Dictionary<string, string>()
            {
                { "sf-expander", "expander" },
                { "sf-style-dropdown", "style-dropdown" },
            };

            foreach (var component in ComponentsDependencyResolver.ComponentsDefinitionsDictionary.Value.Keys)
            {
                if (knownComponentsNamingMismatches.ContainsKey(component))
                    availableComponents.Add(knownComponentsNamingMismatches[component], component);
                else
                    availableComponents.Add(component, component);
            }

            return availableComponents;
        }

        private static readonly Lazy<Dictionary<string, ScriptDependencyConfigModel>> ComponentsDefinitionsDictionary =
           new Lazy<Dictionary<string, ScriptDependencyConfigModel>>(ComponentsDependencyResolver.InitializeComponentsDefinitions, true);

        private static readonly Lazy<Dictionary<string, string>> AvailableComponents =
            new Lazy<Dictionary<string, string>>(ComponentsDependencyResolver.InitializeAvailableComponents, true);
    }
}
