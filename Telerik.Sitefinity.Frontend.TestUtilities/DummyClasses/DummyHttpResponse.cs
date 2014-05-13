using System.Web;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// Fake HTTP response for unit testing.
    /// </summary>
    public class DummyHttpResponse : HttpResponseBase
    {
        /// <summary>
        /// When overridden in a derived class, adds a session ID to the virtual path if the session is using <see cref="P:System.Web.Configuration.SessionStateSection.Cookieless" /> session state, and returns the combined path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of a resource.</param>
        /// <returns>
        /// The virtual path, with the session ID inserted.
        /// </returns>
        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }
    }
}
