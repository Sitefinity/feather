using Ninject.Modules;
using Telerik.Sitefinity.Frontend.Mvc.Models;

namespace Telerik.Sitefinity.Frontend
{
    public class InterfaceMappings : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<IDesignerModel>().To<DesignerModel>();
            Bind<IGridDesignerModel>().To<GridDesignerModel>();
        }
    }
}
