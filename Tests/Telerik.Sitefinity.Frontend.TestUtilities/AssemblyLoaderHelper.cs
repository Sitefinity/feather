using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.Sitefinity.Frontend.TestUtilities
{
    /// <summary>
    /// Helper for common operations related to Asssemblies loading and reflection.
    /// </summary>
    public class AssemblyLoaderHelper
    {
        /// <summary>
        /// Gets the Test Utilities Assembly
        /// </summary>
        /// <returns></returns>
        public static Assembly GetTestUtilitiesAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
