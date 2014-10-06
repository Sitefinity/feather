using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.TestUI.Framework.Framework.Wrappers.Backend.PageTemplates;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.ResourcePackages
{
    /// <summary>
    /// This is test class for Default resource packages tests.
    /// </summary>
    [TestClass]
    public class DefaultResourcePackages : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Owner("Feather team"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void VerifyGeneratedTemplatesDefaultResourcePackages()
        {
            BAT.Macros().User().EnsureAdminLoggedIn();
            BAT.Macros().NavigateTo().Design().PageTemplates();
            Assert.IsTrue(BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().IsItemPresentInGridView(BootstrapTemplate), "template was not found");
            Assert.IsTrue(BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().IsItemPresentInGridView(SemanticTemplate), "template was not found");
            Assert.IsTrue(BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().IsItemPresentInGridView(FoundationTemplate), "template was not found");
        }

        private const string BootstrapTemplate = "Bootstrap.default";
        private const string SemanticTemplate = "SemanticUI.default";
        private const string FoundationTemplate = "Foundation.default";
    }
}
