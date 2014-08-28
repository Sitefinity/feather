using System.Reflection;

namespace Telerik.Sitefinity.Frontend.TestUtilities
{
    /// <summary>
    /// Helper for common operations related to Assemblies loading and reflection.
    /// </summary>
    public static class AssemblyLoaderHelper
    {
        /// <summary>
        /// Gets the Test Utilities Assembly
        /// </summary>
        /// <returns>Assembly context</returns>
        public static Assembly GetTestUtilitiesAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}