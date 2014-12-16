namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve integers.
    /// </summary>
    internal class IntParamResolver : RouteParamResolverBase
    {
        /// <inheritdoc />
        protected override bool TryResolveParamInternal(string urlParam, out object value)
        {
            int intValue;

            if (int.TryParse(urlParam, out intValue))
            {
                value = intValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}