using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace QuickEnumStrings.Tests
{
    /// <remarks>
    /// <see href="https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/">
    /// Credit Andrew Lock
    /// </see>
    /// </remarks>
    internal static class TestHelper
    {
        public static GeneratorDriver GetDriver(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            // Create references for assemblies we require
            // We could add multiple references if required
            IEnumerable<PortableExecutableReference> references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            var generator = new QuickEnumExtensionMethodGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            return driver;
        }

        public static Task Verify(string source)
        {
            var driver = GetDriver(source);

            return Verify(driver);
        }

        public static Task Verify(GeneratorDriver driver)
        {
            return Verifier
                .Verify(driver)
                .UseDirectory("Snapshots");
        }
    }
}
