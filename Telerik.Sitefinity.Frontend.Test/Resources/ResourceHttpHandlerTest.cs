using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Web;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Test.Resources
{
    [TestClass]
    public class ResourceHttpHandlerTest
    {
        #region ProcessRequest

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether ProcessRequest will throw the expected HttpException if the requested resource was not found.")]
        [ExpectedException(typeof(HttpException))]
        public void ProcessRequest_NonExistingFilePath_ThrowsHttpNotFoundException()
        {
            var context = new HttpContext(
               new HttpRequest(null, "http://tempuri.org/test-image.jpg", null),
               new HttpResponse(null));

            var handler = new ResourceHttpHandlerMock();
            handler.FileExistsMock = (p) => false;

            try
            {
                handler.ProcessRequest(context);
            }
            catch (HttpException exception)
            {
                Assert.AreEqual(404, exception.GetHttpCode());
                throw;
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether ProcessRequest will write the content of an existing stylesheet.")]
        public void ProcessRequest_ExistingStylesheet_WritesContentInResponse()
        {
            string stylesContent = "my expected styles";

            var responseWriter = new StringWriter();
            var response = new HttpResponse(responseWriter);
            var context = new HttpContext(
               new HttpRequest(null, "http://tempuri.org/test-style.css", null),
               response);

            var handler = new ResourceHttpHandlerMock();
            handler.FileExistsMock = (p) => true;
            handler.OpenFileMock = (p) =>
                {
                    var str = new MemoryStream();
                    str.Write(Encoding.UTF8.GetBytes(stylesContent), 0, stylesContent.Length);
                    str.Position = 0;
                    return str;
                };

            handler.ProcessRequest(context);
            
            Assert.AreEqual(stylesContent, responseWriter.GetStringBuilder().ToString());
            Assert.AreEqual("text/css", response.ContentType);
        }

        #endregion

        #region Mocks

        private class ResourceHttpHandlerMock : ResourceHttpHandler
        {
            public Func<string, bool> FileExistsMock;
            public Func<string, Stream> OpenFileMock;

            protected override bool FileExists(string path)
            {
                if (this.FileExistsMock != null)
                {
                    return this.FileExistsMock(path);
                }
                else
                {
                    return base.FileExists(path);
                }
            }

            protected override Stream OpenFile(string path)
            {
                if (this.OpenFileMock != null)
                {
                    return this.OpenFileMock(path);
                }
                else
                {
                    return base.OpenFile(path);
                }
            }
        }

        #endregion
    }
}
