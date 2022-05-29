namespace QuickEnumStrings.Tests
{
    [TestFixture]   
    public class AccessModifierTests
    {
        [TestCase("")]
        [TestCase("public")]
        [TestCase("private")]
        [TestCase("protected")]
        [TestCase("internal")]
        public void EnsureValidAccessModifiersGeneratesSource(string accessModifier)
        {
            var enumFileContents = @$"
                using System;
                
                [QuickEnum]
                {accessModifier} enum MyEnum
                {{
                    Bad,
                    Good
                }}";

            var driver = TestHelper.GetDriver(enumFileContents);

            var result = driver.GetRunResult();

            Assume.That(result.Results, Has.Exactly(1).Items);

            var generatorResult = result.Results[0];

            Assume.That(generatorResult.GeneratedSources, Has.Exactly(1).Items,
                "Could not generate source code. Ensure the test's source input is compilable.");

            var generatedSource = generatorResult.GeneratedSources[0];

            Assert.That(generatedSource.SourceText, Is.Not.Null);
        }
    }
}