using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Web;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;

namespace Telerik.Sitefinity.Frontend.Test.Resources
{
    /// <summary>
    /// Ensures that ResourceHttpHandler class works correctly.
    /// </summary>
    [TestClass]
    public class ResourceHttpHandlerTests
    {
        #region ProcessRequest

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether ProcessRequest will throw the expected HttpException if the requested resource was not found.")]
        [ExpectedException(typeof(HttpException))]
        public void ProcessRequest_NonExistingFilePath_ThrowsHttpNotFoundException()
        {
            //Arrange
            var context = new HttpContext(
               new HttpRequest(null, "http://tempuri.org/test-image.jpg", null),
               new HttpResponse(null));

            var handler = new DummyResourceHttpHandler();
            handler.FileExistsMock = (p) => false;

            try
            {
                //Act
                handler.ProcessRequest(context);
            }
            catch (HttpException exception)
            {
                //Assert
                Assert.AreEqual(404, exception.GetHttpCode(), "Http code is not 404");
                throw;
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether ProcessRequest will write the content of an existing stylesheet.")]
        public void ProcessRequest_ExistingStylesheet_WritesContentInResponse()
        {
            //Arrange
            string stylesContent = "my expected styles";

            var responseWriter = new StringWriter();
            var response = new HttpResponse(responseWriter);
            var context = new HttpContext(
               new HttpRequest(null, "http://tempuri.org/test-style.css", null),
               response);

            var handler = new DummyResourceHttpHandler();
            handler.FileExistsMock = (p) => true;
            handler.OpenFileMock = (p) =>
                {
                    var str = new MemoryStream();
                    str.Write(Encoding.UTF8.GetBytes(stylesContent), 0, stylesContent.Length);
                    str.Position = 0;
                    return str;
                };

            //Act
            handler.ProcessRequest(context);
            
            //Assert
            Assert.AreEqual(stylesContent, responseWriter.GetStringBuilder().ToString(), "The expected styles are not retrieved.");
            Assert.AreEqual("text/css", response.ContentType, "The content type of the stylesheets are not correct.");
        }

        #endregion
    }
}
