using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;

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
            : this(new TemplateService(), HostingEnvironment.VirtualPathProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateProcessor"/> class.
        /// </summary>
        /// <param name="templateService">The template service.</param>
        public RazorTemplateProcessor(ITemplateService templateService)
            : this(templateService, HostingEnvironment.VirtualPathProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateProcessor"/> class.
        /// </summary>
        /// <param name="templateService">The template service.</param>
        /// <param name="virtualPathProvider">The virtual path provider for retrieving files.</param>
        public RazorTemplateProcessor(ITemplateService templateService, VirtualPathProvider virtualPathProvider)
        {
            this.service = templateService;
            this.virtualPathProvider = virtualPathProvider;
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

                        var dependency = this.virtualPathProvider.GetCacheDependency(templatePath, null, DateTime.UtcNow);
                        if (dependency != null)
                        {
                            this.templateDependencies[templatePath] = dependency;
                        }
                    }
                }
            }
        }

        private readonly ITemplateService service;
        private readonly VirtualPathProvider virtualPathProvider;
        private readonly Dictionary<string, CacheDependency> templateDependencies = new Dictionary<string, CacheDependency>();
    }
}
