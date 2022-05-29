using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuickEnumStrings.Generators;

/// <remarks>
/// <see href="https://medium.com/c-sharp-progarmming/mastering-at-source-generators-18125a5f3fca">
/// Credit Enis Necipoğlu
/// </see>
/// </remarks>
internal class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver where TAttribute : Attribute
{
    internal IList<EnumDeclarationSyntax> Enums { get; } = new List<EnumDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not EnumDeclarationSyntax enumDeclarationSyntax) { return; }

        var allAttributes = enumDeclarationSyntax.AttributeLists
            .SelectMany(a => a.Attributes);

        // Don't proceed if [Flags] is present
        if (allAttributes.Any(IsFlagAttribute))
        {
            return;
        }

        if (allAttributes.Any(MatchesGenericAttributeTypeName))
        {
            this.Enums.Add(enumDeclarationSyntax);
        }
    }

    private static bool MatchesAttributeName([ReadOnly(true)] AttributeSyntax attr, string attributeName)
    {
        var source = attr.Name.ToString();

        string text;

        // Allow both valid syntaxes
        // i.e. [Custom] & [CustomAttribute] are the same attribute
        if (source.EndsWith("Attribute"))
        {
            text = source;
        }
        else
        {
            text = $"{source}Attribute";
        }

        return text.Equals(attributeName);
    }

    private static bool MatchesGenericAttributeTypeName([ReadOnly(true)] AttributeSyntax attr) => MatchesAttributeName(attr, typeof(TAttribute).Name);

    private const string FlagsAttributeName = nameof(FlagsAttribute);
    private static bool IsFlagAttribute([ReadOnly(true)] AttributeSyntax attr) => MatchesAttributeName(attr, FlagsAttributeName);

}