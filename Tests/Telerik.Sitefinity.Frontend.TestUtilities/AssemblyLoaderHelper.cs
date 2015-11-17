using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web.WebPages;

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

        /// <summary>
        /// Ensures that the razor views are precompiled in a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="failedViews">The views that are found to not be precompiled.</param>
        /// <returns>True if all view are precompiled.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public static bool EnsurePrecompiledRazorViews(Assembly assembly, out string[] failedViews)
        {
            var extension = ".cshtml";
            var prefix = assembly.GetName().Name + ".";
            var names = assembly.GetManifestResourceNames().Where(r => r.EndsWith(extension, StringComparison.OrdinalIgnoreCase)).Select(n => n.ToUpperInvariant());
            var viewTypes = assembly.GetExportedTypes()
                .Select(t => t.GetCustomAttribute<PageVirtualPathAttribute>())
                    .Where(a => a != null)
                    .Select(a => a.VirtualPath)
                        .Select(n => n.Replace("~/", prefix).Replace('/', '.').ToUpperInvariant());

            failedViews = names.Except(viewTypes).ToArray();
            return failedViews.Length == 0;
        }
    }
}