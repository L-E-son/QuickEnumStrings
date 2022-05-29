namespace QuickEnumStrings.Tests
{
    [TestFixture]
    public class MultiValueEnumTests
    {
        [Test]
        public void EnsureDeclarationsWithEqualBackingValuesReturnCorrectValues()
        {
            var enumFileContents = @"
                using System;
                
                [QuickEnum]
                public enum MyEnum
                {
                    One = 1,
                    First = 1
                }";

            var driver = TestHelper.GetDriver(enumFileContents);

            var result = driver.GetRunResult();

            Assume.That(result.Results, Has.Exactly(1).Items);

            var generatorResult = result.Results[0];

            Assert.Multiple(() =>
            {
                Assert.That(generatorResult.Exception, Is.Null, "Generator result should be null.");

                Assert.That(generatorResult.GeneratedSources, Has.Exactly(1).Items,
                    "Code should be generated regardless of the backing value in the enum declaration.");
            });
        }
    }
}
