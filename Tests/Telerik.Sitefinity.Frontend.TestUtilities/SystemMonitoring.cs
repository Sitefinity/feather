using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web;

namespace Telerik.Sitefinity.Frontend.TestUtilities
{
    public static class SystemMonitoring
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static int GetRecompilationCount()
        {
            var t = typeof(HttpApplication).Assembly.GetType("System.Web.Compilation.DiskBuildResultCache");
            var field = t.GetField("s_recompilations", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            return (int)field.GetValue(null);
        }
    }
}
