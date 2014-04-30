using System.Collections.Generic;
using System.Linq;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// Class that implement this interface must observe the resources locations, watch for changes 
    /// and take certain actions depending on the change
    /// </summary>
    public interface IFilesMonitor
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="directoriesInfo">The directories information.</param>
        void Start(Dictionary<string, bool> directoriesInfo);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="path">The file path.</param>
        void Stop(string filePath);
    }
}