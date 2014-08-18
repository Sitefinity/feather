using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    public class BackendWrappersFacade
    {
        public PagesWrapperFacade Pages()
        {
            return new PagesWrapperFacade();
        }

        public WidgetsWrapperFacade Widgets()
        {
            return new WidgetsWrapperFacade();
        }
    }
}
