namespace QuickEnumStrings.Tests
{
    public class EnumFlagTests
    {
        [Test]
        public void EnsureCodeIsNotGeneratedWhenFlagsAttributeExists(
            [Values(AttributePlacement.Above,
                    AttributePlacement.Below,
                    AttributePlacement.InlineLeft,
                    AttributePlacement.InlineRight)] AttributePlacement attributePlacement)
        {
            var enumFileContents = @$"
                using System;
                
                {GetAttributesWithPlacement(attributePlacement)}
                public enum MyEnum
                {{
                    Bad,
                    Good
                }}";

            var driver = TestHelper.GetDriver(enumFileContents);

            var result = driver.GetRunResult();

            Assume.That(result.Results, Has.Exactly(1).Items);

            var generatorResult = result.Results[0];

            Assert.Multiple(() =>
            {
                Assert.That(generatorResult.Exception, Is.Null, "Generator result should be null.");

                Assert.That(generatorResult.GeneratedSources, Has.Exactly(0).Items,
                    "Code should not be generated if the FlagsAttribute exists on the enum declaration.");
            });
        }

        private string GetAttributesWithPlacement(AttributePlacement placement)
        {
            const string FlagsEnum = nameof(FlagsAttribute);
            const string QuickEnum = nameof(QuickEnumAttribute);

            switch (placement)
            {
                case AttributePlacement.Above:
                    {
                        return $@"
                        [{FlagsEnum}]
                        [{QuickEnum}]
                        ";
                    }
                case AttributePlacement.Below:
                    {
                        return $@"
                        [{QuickEnum}]
                        [{FlagsEnum}]
                        ";
                    }
                case AttributePlacement.InlineLeft: return $"[{FlagsEnum}, {QuickEnum}]";
                case AttributePlacement.InlineRight: return $"[{QuickEnum}, {FlagsEnum}]";
                default: throw new ArgumentOutOfRangeException(nameof(placement));
            }
        }

        public enum AttributePlacement
        {
            Above, Below, InlineLeft, InlineRight
        }
    }
}
