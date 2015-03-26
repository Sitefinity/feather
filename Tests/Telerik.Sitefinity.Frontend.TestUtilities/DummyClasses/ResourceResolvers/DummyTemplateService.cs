using System;
using System.Collections.Generic;

using RazorEngine.Templating;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    public sealed class DummyTemplateService : ITemplateService
    {
        public DummyTemplateService(Func<string, string> parseFunc)
        {
            this.parseFunc = parseFunc;
        }

        public void AddNamespace(string ns)
        {
            throw new NotImplementedException();
        }

        public void Compile(string razorTemplate, Type modelType, string cacheName)
        {
            this.cache[cacheName] = razorTemplate;
        }

        public ITemplate CreateTemplate(string razorTemplate, Type templateType, object model)
        {
            throw new NotImplementedException();
        }

        public Type CreateTemplateType(string razorTemplate, Type modelType)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public IEnumerable<Type> CreateTemplateTypes(IEnumerable<string> razorTemplates, IEnumerable<Type> modelTypes, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public IEnumerable<ITemplate> CreateTemplates(IEnumerable<string> razorTemplates, IEnumerable<Type> templateTypes, IEnumerable<object> models, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public RazorEngine.Text.IEncodedStringFactory EncodedStringFactory
        {
            get { throw new NotImplementedException(); }
        }

        public ITemplate GetTemplate(string razorTemplate, object model, string cacheName)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public IEnumerable<ITemplate> GetTemplates(IEnumerable<string> razorTemplates, IEnumerable<object> models, IEnumerable<string> cacheNames, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public bool HasTemplate(string cacheName)
        {
            throw new NotImplementedException();
        }

        public string Parse(string razorTemplate, object model, DynamicViewBag viewBag, string cacheName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, IEnumerable<object> models, IEnumerable<DynamicViewBag> viewBags, IEnumerable<string> cacheNames, bool parallel)
        {
            throw new NotImplementedException();
        }

        public bool RemoveTemplate(string cacheName)
        {
            throw new NotImplementedException();
        }

        public ITemplate Resolve(string cacheName, object model)
        {
            throw new NotImplementedException();
        }

        public string Run(ITemplate template, DynamicViewBag viewBag)
        {
            throw new NotImplementedException();
        }

        public string Run(string cacheName, object model, DynamicViewBag viewBag)
        {
            return this.parseFunc(this.cache[cacheName]);
        }

        public void Dispose()
        {
        }

        private readonly Dictionary<string, string> cache = new Dictionary<string, string>();
        private Func<string, string> parseFunc;
    }
}
