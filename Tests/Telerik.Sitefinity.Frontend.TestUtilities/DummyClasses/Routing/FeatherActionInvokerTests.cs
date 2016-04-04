using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Routing
{
    /// <summary>
    /// This class mocks the <see cref="FeatherActionInvoker"/> class. Used for test purposes only.
    /// </summary>
    internal class FeatherActionInvokerMock : FeatherActionInvoker
    {
        /// <summary>
        /// Public method that calls the base protected GetDefaultParamsMapper.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>Result of the base GetDefaultParamsMapper method.</returns>
        public IUrlParamsMapper GetDefaultParamsMapperPublic(ControllerBase controller)
        {
            return this.GetDefaultParamsMapper(controller);
        }
    }
}
