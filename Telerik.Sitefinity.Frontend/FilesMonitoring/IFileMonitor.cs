using System.Collections.Generic;
using System.Linq;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// Classes that implement this interface must observe the resource locations, watch for changes 
    /// and take certain actions depending on the change.
    /// </summary>
    public interface IFileMonitor
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="directoriesInfo">The directories information.</param>
        void Start(IList<MonitoredDirectory> directoriesInfo);
    }
}