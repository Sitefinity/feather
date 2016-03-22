using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;
using Telerik.Sitefinity.Modules.News.Web.UI;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.TestUnit.GridSystem
{
    /// <summary>
    /// Tests the behavior of the GridControlToolboxFilter.
    /// </summary>
    [TestClass]
    public class GridControlToolboxFilterTests
    {
        [Ignore]
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsSectionVisible returns true when called.")]
        public void IsSectionVisible_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Hybrid);
            var result = filter.IsSectionVisible(null);

            Assert.IsTrue(result, "Grid control toolbox filter should not filter sections.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for null arguments.")]
        public void IsToolVisible_NullArguments_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Hybrid);
            var result = filter.IsToolVisible(null);

            Assert.IsTrue(result, "Grid control toolbox filter should not be filtering on null arguments.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for normal widgets in Pure MVC.")]
        public void IsToolVisible_NewsViewInPureMvc_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Mvc);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(NewsView).AssemblyQualifiedName });

            Assert.IsTrue(result, "Grid control toolbox filter should not be filtering non-layout controls.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for normal widgets in WebForms.")]
        public void IsToolVisible_NewsViewInWebForms_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.WebForms);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(NewsView).AssemblyQualifiedName });

            Assert.IsTrue(result, "Grid control toolbox filter should not be filtering non-layout controls.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for normal widgets in Hybrid.")]
        public void IsToolVisible_NewsViewInHybrid_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Hybrid);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(NewsView).AssemblyQualifiedName });

            Assert.IsTrue(result, "Grid control toolbox filter should not be filtering non-layout controls.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns false for non-grid layout controls in pure MVC mode.")]
        public void IsToolVisible_NonGridLayoutControlInPureMvc_ReturnsFalse()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Mvc);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(LayoutControl).AssemblyQualifiedName });

            Assert.IsFalse(result, "Default layout control should not be visible in Pure MVC.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for non-grid layout controls in WebForms mode.")]
        public void IsToolVisible_NonGridLayoutControlInWebFormsOnly_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.WebForms);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(LayoutControl).AssemblyQualifiedName });

            Assert.IsTrue(result, "Default layout controls should be visible in WebForm mode.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for non-grid layout controls in Hybrid mode.")]
        public void IsToolVisible_NonGridLayoutControlInHybrid_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Hybrid);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(LayoutControl).AssemblyQualifiedName });

            Assert.IsTrue(result, "Default layout controls should be visible in Hybrid mode.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for grid layout controls in pure MVC mode.")]
        public void IsToolVisible_GridLayoutControlInPureMvc_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Mvc);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(GridControl).AssemblyQualifiedName });

            Assert.IsTrue(result, "Grid controls should be visible in Pure MVC.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns false for grid layout controls in WebForms mode.")]
        public void IsToolVisible_GridLayoutControlInWebForms_ReturnsFalse()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.WebForms);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(GridControl).AssemblyQualifiedName });

            Assert.IsFalse(result, "Gid controls should not be visible in WebForms.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether IsToolVisible returns true for grid layout controls in Hybrid mode.")]
        public void IsToolVisible_GridLayoutControlInHybrid_ReturnsTrue()
        {
            var filter = new GridControlToolboxFilter(() => PageTemplateFramework.Hybrid);
            var result = filter.IsToolVisible(new ToolboxItemProxy() { ControlType = typeof(GridControl).AssemblyQualifiedName });

            Assert.IsTrue(result, "Grid controls should be visible in Hybrid.");
        }
    }
}
