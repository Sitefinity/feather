using System.Collections.Generic;
using System.Linq;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.GridSystem;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUnit.GridSystem
{
    /// <summary>
    /// Tests related to the GridSystemInitializer class
    /// </summary>
    [TestClass]
    public class GridSystemInitializerTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add layout control_ existing toolbox section_ verify control is properly added to the toolbox.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridSystemInitializer properly adds a new set of layout controls to the toolbox")]
        public void AddLayoutControl_ExistingToolboxSection_VerifyControlIsProperlyAddedToTheToolbox()
        {
            // Arrange: Initialize the GridSystemInitializer, ToolboxesConfig and add a PageLayouts section, create a layouts section, create a dummy grid controls 
            var initializer = new DummyGridSystemInitializer();
            var fakeTemplatePath = "~/fakeTemplatePath/";
            var dummyData = initializer.PublicCreateLayoutControlsData(fakeTemplatePath);
            var toolboxesConfig = new DummyToolboxesConfig();
            var pageControlsMock = new Toolbox(toolboxesConfig.Toolboxes);
            pageControlsMock.Name = "PageLayouts";
            toolboxesConfig.Toolboxes.Add("PageLayouts", pageControlsMock);
            var htmlLayoutsSection = initializer.PublicCreateToolBoxSection(toolboxesConfig);
            ConfigElementList<ToolboxItem> parentToolboxItem = htmlLayoutsSection.Tools;

            // Act: add the grid controls to the toolbox
            foreach (GridControlData gridControl in dummyData)
            {
                initializer.PublicAddLayoutControl(parentToolboxItem, gridControl);
            }

            // Assert: Verify the newly created controls are properly created
            Assert.AreEqual(dummyData.Count, parentToolboxItem.Count, "Not all grid controls were added to the toolbox.");

            var oneColumnGridControl = dummyData.FirstOrDefault();
            var oneColumnGridToolboxItem = parentToolboxItem.Where<ToolboxItem>(toolboxItem => toolboxItem.Name == oneColumnGridControl.Name).FirstOrDefault();

            Assert.IsNotNull(oneColumnGridToolboxItem, "The grid control was not added to the toolbox.");
            Assert.AreEqual(oneColumnGridControl.Title, oneColumnGridToolboxItem.Title, "The grid control toolbox item has wrong title.");
            Assert.AreEqual(oneColumnGridControl.LayoutTemplatePath, oneColumnGridToolboxItem.LayoutTemplate, "The grid control toolbox item has layout template path.");
        }

        /// <summary>
        /// The create layout controls data_ fake template path_ verify controls are properly created.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridSystemInitializer properly creates a new set of layout controls")]
        public void CreateLayoutControlsData_FakeTemplatePath_VerifyControlsAreProperlyCreated()
        {
            // Arrange: Initialize the GridSystemInitializer, ToolboxesConfig and add a PageLayouts section
            var initializer = new DummyGridSystemInitializer();
            string fakeTemplatePath = "~/fakeTemplatePath/";

            // Act: create the layout controls
            List<GridControlData> dummyData = (List<GridControlData>)initializer.PublicCreateLayoutControlsData(fakeTemplatePath);

            // Assert: Verify the newly created controls are properly created
            Assert.IsNotNull(dummyData, "CreateLayoutControlsData returns null data.");
            Assert.AreEqual(11, dummyData.Count, "CreateLayoutControlsData method should return 11 items.");

            GridControlData oneColumnGridControl = dummyData.Where(layoutControl => layoutControl.Name == "Col1").FirstOrDefault();
            Assert.IsNotNull(oneColumnGridControl, "Grid controls was not created correctly or their names are not expected.");
            Assert.AreEqual("12", oneColumnGridControl.Title, "The grid control has wrong title.");
            Assert.AreEqual(fakeTemplatePath + "grid-12.html", oneColumnGridControl.LayoutTemplatePath, "The grid control has layout template path.");
        }

        /// <summary>
        /// The get or create tool box section_ create fake toolbox section_ verify section is added and has proper data.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridSystemInitializer properly creates a new section for the layout controls")]
        public void GetOrCreateToolBoxSection_CreateFakeToolboxSection_VerifySectionIsAddedAndHasProperData()
        {
            // Arrange: Initialize the GridSystemInitializer, ToolboxesConfig and add a PageLayouts section
            var initializer = new DummyGridSystemInitializer();
            var toolboxesConfig = new DummyToolboxesConfig();
            var pageControlsMock = new Toolbox(toolboxesConfig.Toolboxes);

            pageControlsMock.Name = "PageLayouts";
            toolboxesConfig.Toolboxes.Add("PageLayouts", pageControlsMock);

            // Act: create a new toolbox section for the layout controls
            var htmlLayoutsSection = initializer.PublicCreateToolBoxSection(toolboxesConfig);

            // Assert: Verify the newly created section exists and has proper name and title
            Assert.IsNotNull(htmlLayoutsSection, "The toolbox section was not properly created.");
            Assert.AreEqual("HtmlLayouts", htmlLayoutsSection.Name, "The toolbox section has unexpected name.");
            Assert.AreEqual("Bootstrap grid widgets", htmlLayoutsSection.Title, "The toolbox section has unexpected title.");
        }

        #endregion
    }
}