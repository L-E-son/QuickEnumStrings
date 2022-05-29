using System.Text;

namespace QuickEnumStrings.Tests
{
    [TestFixture]
    public class MultiDeclarationFileTests
    {
        [Test]
        public void EnsureCorrectNumberOfFilesAreGenerated(
            [Values(true, false)] bool useAttributeFirstEnum,
            [Values(true, false)] bool useAttributeSecondEnum,
            [Values(true, false)] bool useAttributeThirdEnum)
        {
            var stringBuilder = new StringBuilder("using System;");

            var trueCount = 0;

            string CreateEnumDeclaration(bool useAttribute)
            {
                var enumOrNothing = useAttribute
                    ? "[QuickEnum]"
                    : string.Empty;

                if (useAttribute)
                {
                    trueCount++;
                }

                var enumDeclaration = @$"

                {enumOrNothing}
                public enum MyEnum{Guid.NewGuid():N}
                {{
                    Bad,
                    Good
                }}";

                return enumDeclaration;
            }

            stringBuilder.AppendLine(CreateEnumDeclaration(useAttributeFirstEnum));
            stringBuilder.AppendLine(CreateEnumDeclaration(useAttributeSecondEnum));
            stringBuilder.AppendLine(CreateEnumDeclaration(useAttributeThirdEnum));

            var generatedClass = stringBuilder.ToString();

            var driver = TestHelper.GetDriver(generatedClass);

            var result = driver.GetRunResult();

            Assume.That(result.Results, Has.Exactly(1).Items);

            var generatorResult = result.Results[0];

            Assert.That(generatorResult.GeneratedSources, Has.Exactly(trueCount).Items);
        }
    }
}
