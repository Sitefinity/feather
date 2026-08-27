using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.Events;

namespace Telerik.Sitefinity.Frontend.SchemaMarkup
{
    internal class SchemaMarkupInitializer : IInitializer
    {
        public void Initialize()
        {
            EventHub.Subscribe<IPagePreRenderCompleteEvent>(SchemaMarkupInjector.OnPagePreRenderCompleteEventHandler);
        }

        public void Uninitialize()
        {
            EventHub.Unsubscribe<IPagePreRenderCompleteEvent>(SchemaMarkupInjector.OnPagePreRenderCompleteEventHandler);
        }
    }
}
