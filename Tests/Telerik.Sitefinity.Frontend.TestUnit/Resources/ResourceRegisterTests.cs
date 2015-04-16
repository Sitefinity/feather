using System;
using System.IO;
using System.Linq;
using System.Web;

using global::Microsoft.VisualStudio.TestTools.UnitTesting;

using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    /// <summary>
    ///     This class contains tests that ensure that the <see cref="ResourceRegister" /> works properly
    /// </summary>
    [TestClass]
    public class ResourceRegisterTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Ignore]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that exception is thrown in case that resource can be registered successfully.")]
        [ExpectedException(typeof(ArgumentException), "There should be exception regarding the duplication of the resource registration!")]
        public void RegisterResource_AlreadyRegisteredResource_ExceptionIsThrown()
        {
            // Arrange
            string registerName = "TestRegister";
            HttpContextBase context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            string fakeResourceKey = "test-resource";
            register.Register(fakeResourceKey);
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 1);

            // Act
            register.Register(fakeResourceKey, throwException: true);

            // Assert
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 1);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that a resource can be registered successfully.")]
        public void RegisterResource_NewResource_ResourceIsRegistered()
        {
            // Arrange
            string registerName = "TestRegister";
            HttpContextBase context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            string fakeResourceKey = "test-resource";
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 0);

            // Act
            register.Register(fakeResourceKey);

            // Assert
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 1);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that resource will not be registered again if it is already registered.")]
        public void TryRegisterResource_AlreadyRegisteredResource_ResourceIsNotRegisteredTwice()
        {
            // Arrange
            string registerName = "TestRegister";
            HttpContextBase context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            string fakeResourceKey = "test-resource";
            register.Register(fakeResourceKey);
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 1);

            // Act
            bool result = register.Register(fakeResourceKey);

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 1);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that a resource can be successfully registered.")]
        public void TryRegisterResource_NewResource_ResourceIsRegistered()
        {
            // Arrange
            string registerName = "TestRegister";
            HttpContextBase context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            string fakeResourceKey = "test-resource";
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 0);

            // Act
            bool result = register.Register(fakeResourceKey);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(register.GetInlineResources().Count(i => i == fakeResourceKey) == 1);
        }

        #endregion

        #region Methods

        private HttpContextBase CreateHttpContext()
        {
            var request = new HttpRequest(string.Empty, "http://tempuri.org", string.Empty);
            var response = new HttpResponse(new StringWriter(System.Globalization.CultureInfo.InvariantCulture));
            var httpContext = new HttpContext(request, response);
            var result = new HttpContextWrapper(httpContext);

            return result;
        }

        #endregion
    }
}