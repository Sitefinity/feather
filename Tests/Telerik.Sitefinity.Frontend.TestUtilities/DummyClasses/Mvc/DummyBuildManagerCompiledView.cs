using System.IO;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Views
{
    /// <summary>
    /// This class inherits <see cref="System.Web.Mvc.BuildManagerCompiledView"/> for test purposes.
    /// </summary>
    public class DummyBuildManagerCompiledView : BuildManagerCompiledView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyBuildManagerCompiledView"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The view path.</param>
        public DummyBuildManagerCompiledView(ControllerContext controllerContext, string viewPath)
            : base(controllerContext, viewPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyBuildManagerCompiledView"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="viewPageActivator">The view page activator.</param>
        protected DummyBuildManagerCompiledView(ControllerContext controllerContext, string viewPath, IViewPageActivator viewPageActivator)
            : base(controllerContext, viewPath, viewPageActivator)
        {
        }

        /// <summary>
        /// When overridden in a derived class, renders the specified view context by using the specified writer object and object instance.
        /// </summary>
        /// <param name="viewContext">Information related to rendering a view, such as view data, temporary data, and form context.</param>
        /// <param name="writer">The writer object.</param>
        /// <param name="instance">An object that contains additional information that can be used in the view.</param>
        protected override void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {
        }
    }
}
