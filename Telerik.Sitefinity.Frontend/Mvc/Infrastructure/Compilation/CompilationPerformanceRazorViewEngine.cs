using System.Reflection;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Compilation
{
    /// <summary>
    /// This class represents a <see cref="RazorViewEngine"/> which creates <see cref="IView"/> objects allowing measurement of their compilation performance.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.RazorViewEngine" />
    internal class CompilationPerformanceRazorViewEngine : RazorViewEngine
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationPerformanceRazorViewEngine"/> class.
        /// </summary>
        public CompilationPerformanceRazorViewEngine()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationPerformanceRazorViewEngine"/> class.
        /// </summary>
        /// <param name="pageActivator">The page activator.</param>
        public CompilationPerformanceRazorViewEngine(IViewPageActivator pageActivator)
            : base(pageActivator)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a view by using the specified controller context and the paths of the view and master view.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The path to the view.</param>
        /// <param name="masterPath">The path to the master view.</param>
        /// <returns>
        /// The view.
        /// </returns>
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var view = new CompilationPerformanceRazorView(controllerContext, viewPath, masterPath, true, this.FileExtensions, this.ViewPageActivator);
            displayModeProviderProperty.SetValue(view, this.DisplayModeProvider);

            return view;
        }

        /// <summary>
        /// Creates a partial view using the specified controller context and partial path.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="partialPath">The path to the partial view.</param>
        /// <returns>
        /// The partial view.
        /// </returns>
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var view = new CompilationPerformanceRazorView(controllerContext, partialPath, null, false, this.FileExtensions, this.ViewPageActivator);
            displayModeProviderProperty.SetValue(view, this.DisplayModeProvider);

            return view;
        }

        #endregion

        #region Fields

        private static PropertyInfo displayModeProviderProperty = typeof(RazorView).GetProperty("DisplayModeProvider", BindingFlags.Instance | BindingFlags.NonPublic);

        #endregion
    }
}
