using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Test.DummyClasses;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Configuration;


namespace Telerik.Sitefinity.Frontend.Test.GridSystemTests
{
    /// <summary>
    /// Tests related to the GridSystemInitializer class
    /// </summary>
    [TestClass]
    public class GridSystemInitializerTests
    {
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridSystemInitializer properly creates a new section for the layout controls")]
        public void GetOrCreateToolBoxSection_CreateFakeToolboxSection_VerifySectionIsAddedAndHasProperData()
        {
            //Arrange: Initialize the GridSystemInitializer, ToolboxesConfig and add a PageLayouts section
            DummyGridSystemInitializer initializer = new DummyGridSystemInitializer();
            var toolboxesConfig = new DummyToolboxesConfig();
            var pageControlsMock = new Toolbox(toolboxesConfig.Toolboxes);
            pageControlsMock.Name = "PageLayouts";
            toolboxesConfig.Toolboxes.Add("PageLayouts", pageControlsMock);

            //Act: create a new toolbox section for the layout controls
            var htmlLayoutsSection = initializer.PublicCreateToolBoxSection(toolboxesConfig);

            //Assert: Verify the newly created section exists and has proper name and title
            Assert.IsNotNull(htmlLayoutsSection, "The toolbox section was not properly created.");
            Assert.AreEqual<string>("HtmlLayouts", htmlLayoutsSection.Name,"The toolbox section has unexpected name.");
            Assert.AreEqual<string>("Bootstrap grid widgets", htmlLayoutsSection.Title, "The toolbox section has unexpected title.");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridSystemInitializer properly creates a new set of layout controls")]
        public void CreateLayoutControlsData_FakeTemplatePath_VerifyControlsAreProperlyCreated()
        {
            //Arrange: Initialize the GridSystemInitializer, ToolboxesConfig and add a PageLayouts section
            DummyGridSystemInitializer initializer = new DummyGridSystemInitializer();
            var fakeTemplatePath = "~/fakeTemplatePath/";

            //Act: create the layout controls
            var dummyData = initializer.PublicCreateLayoutControlsData(fakeTemplatePath);

            //Assert: Verify the newly created controls are properly created
            Assert.IsNotNull(dummyData);
            Assert.AreEqual<int>(10, dummyData.Count);

            var oneColumnGridControl = dummyData.Where(layoutControl => layoutControl.Name == "Col1").FirstOrDefault();
            Assert.IsNotNull(oneColumnGridControl, "Grid controls was not created correctly or their names are not expected.");
            Assert.AreEqual<string>("12", oneColumnGridControl.Title, "The grid control has wrong title.");
            Assert.AreEqual<string>(fakeTemplatePath + "grid-12.html", oneColumnGridControl.LayoutTemplatePath, "The grid control has layout template path.");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridSystemInitializer properly adds a new set of layout controls to the toolbox")]
        public void AddLayoutControl_ExistingToolboxSection_VerifyControlIsProperlyAddedToTheToolbox()
        {
            //Arrange: Initialize the GridSystemInitializer, ToolboxesConfig and add a PageLayouts section, create a layouts section, create a dummy grid controls 
            DummyGridSystemInitializer initializer = new DummyGridSystemInitializer();
            var fakeTemplatePath = "~/fakeTemplatePath/";
            var dummyData = initializer.PublicCreateLayoutControlsData(fakeTemplatePath);
            var toolboxesConfig = new DummyToolboxesConfig();
            var pageControlsMock = new Toolbox(toolboxesConfig.Toolboxes);
            pageControlsMock.Name = "PageLayouts";
            toolboxesConfig.Toolboxes.Add("PageLayouts", pageControlsMock);
            var htmlLayoutsSection = initializer.PublicCreateToolBoxSection(toolboxesConfig);
            var parentToolboxItem = htmlLayoutsSection.Tools;

            //Act: add the grid controls to the toolbox
            foreach (var gridControl in dummyData)
            {
                initializer.PublicAddLayoutControl(parentToolboxItem, gridControl);
            }

            //Assert: Verify the newly created controls are properly created
            Assert.AreEqual<int>(dummyData.Count, parentToolboxItem.Count, "Not all grid controls were added to the toolbox.");

            var oneColumnGridControl = dummyData.FirstOrDefault();

            var oneColumnGridToolboxItem = parentToolboxItem.Where<ToolboxItem>(toolboxItem => toolboxItem.Name == oneColumnGridControl.Name).FirstOrDefault();

            Assert.IsNotNull(oneColumnGridToolboxItem, "The grid control was not added to the toolbox.");
            Assert.AreEqual<string>(oneColumnGridControl.Title, oneColumnGridToolboxItem.Title, "The grid control toolbox item has wrong title.");
            Assert.AreEqual<string>(oneColumnGridControl.LayoutTemplatePath, oneColumnGridToolboxItem.LayoutTemplate, "The grid control toolbox item has layout template path.");
        }

    }
}
