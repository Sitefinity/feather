using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.Sitefinity.Frontend.TestUtilities
{
    public static class FileInjectHelper
    {
        public static Assembly GetArrangementsAssembly()
        {
            var uiTestsArrangementsAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Equals("Telerik.Sitefinity.Frontend.TestUI.Arrangements")).FirstOrDefault();
            if (uiTestsArrangementsAssembly == null)
            {
                throw new DllNotFoundException("Arrangements assembly wasn't found");
            }

            return uiTestsArrangementsAssembly;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        public static string GetDestinationFilePath(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            var sfpath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            var fullPath = Path.Combine(sfpath, filePath);

            return fullPath;
        }
    }
}
