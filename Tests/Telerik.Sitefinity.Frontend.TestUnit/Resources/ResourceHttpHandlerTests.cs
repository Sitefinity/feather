using System.IO;
using System.Text;
using System.Web;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    /// <summary>
    /// Ensures that ResourceHttpHandler class works correctly.
    /// </summary>
    [TestClass]
    public class ResourceHttpHandlerTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether ProcessRequest will write the content of an existing stylesheet.")]
        public void ProcessRequest_ExistingStylesheet_WritesContentInResponse()
        {
            // Arrange
            string stylesContent = "my expected styles";

            var outputStream = new MemoryStream();
            var response = new HttpResponse(new StringWriter(System.Globalization.CultureInfo.InvariantCulture));
            var context = new HttpContext(new HttpRequest(null, "http://tempuri.org/test-style.css", null), response);

            var handler = new DummyResourceHttpHandler(string.Empty);
            handler.FileExistsMock = p => true;
            handler.OpenFileMock = p =>
                {
                    var str = new MemoryStream();
                    str.Write(Encoding.UTF8.GetBytes(stylesContent), 0, stylesContent.Length);
                    str.Position = 0;
                    return str;
                };
            handler.WriteToOutputMock = (ctx, buffer) =>
                {
                    outputStream.Write(buffer, 0, buffer.Length);
                    outputStream.Position = 0;
                };

            // Act
            handler.ProcessRequest(context);

            string responseText;
            using (var reader = new StreamReader(outputStream))
            {
                responseText = reader.ReadToEnd();
            }

            // Assert
            Assert.AreEqual(stylesContent, responseText, "The expected styles are not retrieved.");
            Assert.AreEqual("text/css", response.ContentType, "The content type of the stylesheets are not correct.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether ProcessRequest will throw the expected HttpException if the requested resource was not found.")]
        [ExpectedException(typeof(HttpException))]
        public void ProcessRequest_NonExistingFilePath_ThrowsHttpNotFoundException()
        {
            // Arrange
            var context = new HttpContext(
                new HttpRequest(null, "http://tempuri.org/test-image.jpg", null), 
                new HttpResponse(null));

            var handler = new DummyResourceHttpHandler(string.Empty);
            handler.FileExistsMock = p => false;

            try
            {
                // Act
                handler.ProcessRequest(context);
            }
            catch (HttpException exception)
            {
                // Assert
                Assert.AreEqual(404, exception.GetHttpCode(), "Http code is not 404");
                throw;
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether a whitelisted sf-cshtml file will be parsed.")]
        public void ProcessRequest_WhitelistedRazorTemplated_Parsed()
        {
            // Arrange
            bool isParsed = false;

            var response = new HttpResponse(new StringWriter(System.Globalization.CultureInfo.InvariantCulture));
            var context = new HttpContext(new HttpRequest(null, "http://tempuri.org/template.sf-cshtml", null), response);

            var handler = new DummyResourceHttpHandler(string.Empty);
            handler.FileExistsMock = p => true;
            handler.IsWhitelistedMock = p => true;
            handler.SendParsedTemplateMock = ctx =>
            {
                isParsed = true;
            };

            // Act
            handler.ProcessRequest(context);

            // Assert
            Assert.IsTrue(isParsed, "The template was not parsed.");
        }

        #endregion
    }
}