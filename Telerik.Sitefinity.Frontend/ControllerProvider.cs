using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Mvc.Store;

namespace Telerik.Sitefinity.Frontend
{
    internal class ControllerProvider : IControllerProvider
    {
        public IEnumerable<Type> GetControllers()
        {
            var controllerContainerInitializer = new ControllerContainerInitializer();
            return controllerContainerInitializer.GetControllers();
        }
    }
}
