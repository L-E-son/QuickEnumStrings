namespace QuickEnumStrings
{
    internal static class Templates
    {
        internal static readonly string SourceCodeTemplate =
@"using System;

namespace {0}
{{
    public static partial class QuickEnumMethods
    {{
        {2}static string AsString(this {1} enumValue)
        {{
            switch (enumValue)
            {{  
                {3}
                default:
                    throw new ArgumentOutOfRangeException(nameof(enumValue), ""Enum value out of range."");

            }}
        }}
    }}
}}";

        internal static readonly string SwitchCaseTemplate = @"
                case {0}:
                    return nameof({0});";
    }
}
