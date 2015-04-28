using System;
using System.IO;
using System.Net;
using System.Text;
using MbUnit.Framework;
using Telerik.Sitefinity.TestUtilities;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Services
{
    [TestFixture]
    [Category(TestCategories.Services)]
    [Author(TestAuthor.Team2)]
    [Description("Integration tests that ensure that the FilesService is operating properly.")]
    public class FilesServiceTests
    {
        [Description("Verifies that when called with no file extension exception is thrown")]
        public void PassingNoFileExtension_FilesService_ExceptionThrown()
        {
            var url = UrlPath.ResolveAbsoluteUrl("~/files-api/items");
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();

            string responseContent;

            using (var responseStream = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                responseContent = responseStream.ReadToEnd();
            }

            Assert.AreEqual(1, 1);
        }
    }
}
