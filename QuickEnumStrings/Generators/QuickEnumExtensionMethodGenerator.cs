using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickEnumStrings.Generators
{
    [Generator]
    internal class QuickEnumExtensionMethodGenerator : ISourceGenerator
    {
        private const string QuickEnumAttributeName = "QuickEnum";

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not AttributeSyntaxReceiver<QuickEnumAttribute> syntaxReceiver)
            {
                return;
            }
            
            System.Diagnostics.Debug.WriteLine("Initalize code generator");

            try
            {
                var markedEnumTypes = syntaxReceiver.Enums.Select(e => new EnumMetadata(e));

                if (!markedEnumTypes.Any())
                {
                    return;
                }

                foreach (var markedEnum in markedEnumTypes)
                {
                    var generatedClass = ConstructGeneratedMethodClass(markedEnum);

                    context.AddSource($"{markedEnum.EnumName}.g.cs", generatedClass);
                }

                System.Diagnostics.Debug.WriteLine("Code generation completed without errors.");
            }
            catch (Exception)
            {
                ;
            }
        }

        private static string ConstructGeneratedMethodClass(EnumMetadata enumMetadata)
        {
            var nameSpace = enumMetadata.Namespace;
            var access = enumMetadata.EnumAccessModifier;
            var typeName = enumMetadata.EnumName;

            return string.Format(
                Templates.SourceCodeTemplate,
                nameSpace,
                typeName,
                access,
                BuildKnownSwitchCases(enumMetadata));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() =>
                new AttributeSyntaxReceiver<QuickEnumAttribute>());
        }

        private static IEnumerable<EnumMetadata> GetAllMarkedEnums(Compilation compilation)
        {
            var syntaxNodes = compilation.SyntaxTrees.SelectMany(sn => sn.GetRoot().DescendantNodes());
            var allEnumDeclarations = syntaxNodes.Where(sn => sn.IsKind(SyntaxKind.EnumDeclaration))
                                                 .OfType<EnumDeclarationSyntax>();

            var QuickEnumEnum = allEnumDeclarations.Where(d => HasQuickEnumAttribute(d));

            return QuickEnumEnum.Select(e => new EnumMetadata(e));
        }

        private static bool HasQuickEnumAttribute(EnumDeclarationSyntax enumDeclaration)
        {
            var attrs = enumDeclaration.AttributeLists
                .SelectMany(a => a.Attributes);

            return attrs.Any(attr => QuickEnumAttributeName.Equals(attr.Name.ToString()));
        }

        private static string BuildKnownSwitchCases(EnumMetadata enumMetadata)
        {
            var builder = new StringBuilder();

            var enumName = enumMetadata.EnumName;

            foreach (var enumVal in enumMetadata.EnumValues)
            {
                builder.AppendFormat(Templates.SwitchCaseTemplate, $"{enumName}.{enumVal}");
            }

            return builder.ToString();
        }

        private sealed class EnumMetadata
        {
            public string Namespace { get; }

            public string EnumAccessModifier { get; }

            public string EnumName { get; }

            public IEnumerable<string> EnumValues { get; }

            public EnumMetadata(EnumDeclarationSyntax enumDeclarationSyntax)
            {
                this.Namespace = GetClosestNamespaceDeclaration(enumDeclarationSyntax)?.Name?.ToString() ?? string.Empty; // TODO: test against nested namespaces
                this.EnumName = enumDeclarationSyntax.Identifier.ValueText;
                this.EnumValues = enumDeclarationSyntax.Members.Select(m => m.Identifier.ValueText);

                if (enumDeclarationSyntax.Modifiers.Count == 0)
                {
                    this.EnumAccessModifier = String.Empty;
                }
                else if (enumDeclarationSyntax.Modifiers.Count == 1)
                {
                    this.EnumAccessModifier = $"{enumDeclarationSyntax.Modifiers[0].ValueText} ";
                }
                else
                {
                    throw new Exception("Unhandled scenario determining access modifier.");
                }
            }

            private static NamespaceDeclarationSyntax? GetClosestNamespaceDeclaration(SyntaxNode? syntaxNode)
            {
                if (syntaxNode is null) return null;

                if (syntaxNode is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    return namespaceDeclaration;
                }

                // TODO: test speed of this to speed of INamespaceSymbol.ContainingNamespace
                return GetClosestNamespaceDeclaration(syntaxNode.Parent);
            }
        }
    }
}
