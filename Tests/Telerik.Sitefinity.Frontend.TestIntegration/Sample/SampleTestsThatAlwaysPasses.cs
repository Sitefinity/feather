using System;
using MbUnit.Framework;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Sample
{
    [TestFixture]
    [Description("This is a sample class with a test that always passes.")]
    public class SampleTestsThatAlwaysPasses
    {
        [Test]
        [Category(TestCategories.Samples)]
        [Author("idimitrov")]
        public void SampleTestThatAlwaysPasses()
        {
            var expected = new Guid("28AABFCA-6FFF-49FD-96C1-B7C1023DAE7A");
            var actual = new Guid("28AABFCA-6FFF-49FD-96C1-B7C1023DAE7A");
            Assert.AreEqual(expected, actual);
        }
    }
}
