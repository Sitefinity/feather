using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// Instance of this class replaces resources in a given template with their localized values.
    /// The instance provides in-memory cache for all parsed templates.
    /// A template's cache is invalidated when the file with the given file path is changed and also when the app is restarted.
    /// </summary>
    public class ResourceTemplateProcessor
    {
        #region Construction
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTemplateProcessor" /> class.
        /// </summary>
        public ResourceTemplateProcessor() : this(HostingEnvironment.VirtualPathProvider)
        {
            this.cachedTemplates = new Dictionary<string, CachedTemplate>();
            this.resourceRegex = new Regex(ResourceTemplateProcessor.ResourcePattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTemplateProcessor" /> class.
        /// </summary>
        /// <param name="virtualPathProvider">The virtual path provider for retrieving files.</param>
        public ResourceTemplateProcessor(VirtualPathProvider virtualPathProvider)
        {
            this.virtualPathProvider = virtualPathProvider;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Localizes the template at the specified path.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <returns>The localized template.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public string Process(string templatePath)
        {
            if (this.ShouldProcessTemplate(templatePath))
            {
                lock (this.cachedTemplates)
                {
                    if (this.ShouldProcessTemplate(templatePath))
                    {
                        try
                        {
                            var markup = this.GetTemplate(templatePath);
                            var processedTemplate = this.ReplaceResources(markup);

                            var dependency = this.virtualPathProvider.GetCacheDependency(templatePath, null, DateTime.UtcNow);

                            this.cachedTemplates[templatePath] = new CachedTemplate()
                            {
                                Dependency = dependency,
                                Template = processedTemplate
                            };

                            return processedTemplate;
                        }
                        catch (Exception ex)
                        {
                            Log.Write(string.Format("Error when localizing a template with path {0}. Exception: {1}", templatePath, ex.ToString()));
                            return ex.Message;
                        }
                    }
                    else
                    {
                        return this.cachedTemplates[templatePath].Template;
                    }
                }
            }
            else
            {
                return this.cachedTemplates[templatePath].Template;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Replaces the resources in the given markup with their respective localized values.
        /// </summary>
        /// <param name="markup">The markup.</param>
        /// <returns></returns>
        private string ReplaceResources(string markup)
        {
            return this.resourceRegex.Replace(
                markup,
                (Match match) =>
                {
                    var resClass = match.Groups["ResourceClass"].Value;
                    var resKey = match.Groups["PropertyName"].Value;
                    return Res.Get(resClass, resKey);
                });
        }

        /// <summary>
        /// Gets the template from a file at the specified path as string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The template as string.</returns>
        private string GetTemplate(string path)
        {
            var templateText = string.Empty;

            if (this.virtualPathProvider.FileExists(path))
            {
                var fileStream = this.virtualPathProvider.GetFile(path).Open();

                using (var streamReader = new StreamReader(fileStream))
                {
                    templateText = streamReader.ReadToEnd();
                }
            }

            return templateText;
        }

        /// <summary>
        /// Determines whether the template should be parsed - when the file is changed or if it is not in the cache.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <returns></returns>
        private bool ShouldProcessTemplate(string templatePath)
        {
            return !this.cachedTemplates.ContainsKey(templatePath) ||
                (this.cachedTemplates[templatePath].Dependency != null && this.cachedTemplates[templatePath].Dependency.HasChanged);
        }

        #endregion

        #region Private fields and constants
        private readonly Dictionary<string, CachedTemplate> cachedTemplates;
        private readonly VirtualPathProvider virtualPathProvider;
        private readonly Regex resourceRegex;
        private const string ResourcePattern = @"@\(\s*Res.Get<(?<ResourceClass>[\w_\d]+)>\(\)\.(?<PropertyName>[\w_\d]+)\s*\)";
        #endregion

        #region Nested classes
        private class CachedTemplate
        {
            public string Template { get; set; }

            public CacheDependency Dependency { get; set; }
        }
        #endregion
    }
}