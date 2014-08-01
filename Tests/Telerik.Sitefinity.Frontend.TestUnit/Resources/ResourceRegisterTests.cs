using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Test.Resources
{
    /// <summary>
    /// This class contains tests that ensure that the <see cref="ResourceRegister"/> works properly
    /// </summary>
    [TestClass]
    public class ResourceRegisterTests
    {
        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that a resource can be registered successfully.")]
        public void RegisterResource_NewResource_ResourceIsRegistered()
        {
            //Arrange
            var registerName = "TestRegister";
            var context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            var fakeResourceKey = "test-resource";
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 0);

            //Act
            register.RegisterResource(fakeResourceKey);

            //Assert
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 1);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that exception is thrown in case that resource can be registered successfully.")]
        [ExpectedException(typeof(ArgumentException), "There should be exception regarding the duplication of the resource registration!")]
        public void RegisterResource_AlreadyRegisteredResource_ExceptionIsThrown()
        {
            //Arrange
            var registerName = "TestRegister";
            var context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            var fakeResourceKey = "test-resource";
            register.RegisterResource(fakeResourceKey);
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 1);

            //Act
            register.RegisterResource(fakeResourceKey);

            //Assert
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 1);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that a resource can be successfully registered.")]
        public void TryRegisterResource_NewResource_ResourceIsRegistered()
        {
            //Arrange
            var registerName = "TestRegister";
            var context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            var fakeResourceKey = "test-resource";
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 0);

            //Act
            var result = register.TryRegisterResource(fakeResourceKey);

            //Assert
            Assert.IsTrue(result);
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 1);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Verifies that resource will not be registered again if it is already registered.")]
        public void TryRegisterResource_AlreadyRegisteredResource_ResourceIsNotRegisteredTwice()
        {
            //Arrange
            var registerName = "TestRegister";
            var context = this.CreateHttpContext();
            var register = new ResourceRegister(registerName, context);

            var fakeResourceKey = "test-resource";
            register.RegisterResource(fakeResourceKey);
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 1);

            //Act
            var result = register.TryRegisterResource(fakeResourceKey);
            
            //Assert
            Assert.IsFalse(result);
            Assert.IsTrue(register.Container.Count(i => i == fakeResourceKey) == 1);
        }

        private System.Web.HttpContextBase CreateHttpContext()
        {
            var request = new System.Web.HttpRequest("", "http://tempuri.org", "");
            var response = new System.Web.HttpResponse(new System.IO.StringWriter());
            var httpContext = new System.Web.HttpContext(request, response);
            var result = new System.Web.HttpContextWrapper(httpContext);

            return result;
        }
    }
}
