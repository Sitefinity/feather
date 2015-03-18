using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;

using RazorEngine;
using RazorEngine.Templating;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// Instances of this class run and compile Razor template files and return the output. It also manages cache dependencies on the template internally.
    /// </summary>
    public class RazorTemplateProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateProcessor"/> class.
        /// </summary>
        public RazorTemplateProcessor()
            : this(new TemplateService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateProcessor"/> class.
        /// </summary>
        /// <param name="templateService">The template service.</param>
        public RazorTemplateProcessor(ITemplateService templateService)
        {
            this.service = templateService;
        }

        /// <summary>
        /// Runs the Razor template that is on the specified path.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <param name="model">The model.</param>
        /// <returns>The output of the template. If compilation error occurs it will be returned and NOT thrown.</returns>
        public string Run(string templatePath, object model)
        {
            return this.Run(templatePath, model, throwOnError: false);
        }

        /// <summary>
        /// Runs the specified template path.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <param name="model">The model.</param>
        /// <param name="throwOnError">if set to <c>true</c> compilation errors will be thrown otherwise the errors will be returned as string.</param>
        /// <returns>The output of the template.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public string Run(string templatePath, object model, bool throwOnError)
        {
            if (throwOnError)
            {
                return this.CompileAndRun(templatePath, model);
            }
            else
            {
                try
                {
                    return this.CompileAndRun(templatePath, model);
                }
                catch (TemplateCompilationException ex)
                {
                    if (ex.Errors != null && ex.Errors.Count > 0)
                        return ex.Errors[ex.Errors.Count - 1].ToString();
                    else
                        return ex.ToString();
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the cache dependency for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A cache dependency.</returns>
        protected virtual CacheDependency GetCacheDependency(string path)
        {
            return HostingEnvironment.VirtualPathProvider.GetCacheDependency(path, null, DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the template from a file at the specified path as string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The template as string.</returns>
        protected virtual string GetTemplate(string path)
        {
            var templateText = string.Empty;

            if (HostingEnvironment.VirtualPathProvider.FileExists(path))
            {
                var fileStream = VirtualPathProvider.OpenFile(path);

                using (var streamReader = new StreamReader(fileStream))
                {
                    templateText = streamReader.ReadToEnd();
                }
            }

            return templateText;
        }

        private string CompileAndRun(string templatePath, object model)
        {
            this.EnsureTemplateIsCompiled(templatePath);
            return this.service.Run(templatePath, model, null);
        }

        private void EnsureTemplateIsCompiled(string templatePath)
        {
            if (!this.templateDependencies.ContainsKey(templatePath) || this.templateDependencies[templatePath].HasChanged)
            {
                lock (this.templateDependencies)
                {
                    if (!this.templateDependencies.ContainsKey(templatePath) || this.templateDependencies[templatePath].HasChanged)
                    {
                        var markup = this.GetTemplate(templatePath);
                        this.service.Compile(markup, null, templatePath);

                        this.templateDependencies[templatePath] = this.GetCacheDependency(templatePath);
                    }
                }
            }
        }

        private readonly ITemplateService service;
        private readonly Dictionary<string, CacheDependency> templateDependencies = new Dictionary<string, CacheDependency>();
    }
}
