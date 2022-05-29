using System.Text;

namespace QuickEnumStrings.Tests
{
    internal class LargeEnumDeclarationTests
    {
        [Ignore("TODO")]
        [Test]
        public void EnsureGeneratorCanHandleLargeEnums()
        {
            var enumFileBuilder = new StringBuilder(@"
                using System;
                
                [QuickEnum]
                public enum MyEnum : ulong
                {
");

            for (var i = ulong.MinValue; i <= ulong.MaxValue; i++)
            {
                enumFileBuilder.AppendFormat("{0} = {1},", Guid.NewGuid().ToString("N"), i)
                               .AppendLine();
            }

            enumFileBuilder.Append("}");

            var driver = TestHelper.GetDriver(enumFileBuilder.ToString());

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
