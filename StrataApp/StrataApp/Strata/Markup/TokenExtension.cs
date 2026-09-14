using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace StrataApp.Strata.Markup;

/// <summary>
/// Custom markup extension: {strata:Token Name=Surface.Container}
///
/// Resolves a semantic token name against the application resources, with an
/// optional Fallback so a missing token fails loudly in the UI instead of
/// throwing at parse time.
///
/// Resolution happens once, when the XAML is parsed. For live theme swapping,
/// point the token at a ThemeResource-backed brush rather than a literal.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(object))]
public partial class TokenExtension : MarkupExtension
{
    public string Name { get; set; } = string.Empty;

    public object? Fallback { get; set; }

    protected override object? ProvideValue()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return Fallback;
        }

        var resources = Application.Current?.Resources;

        if (resources is not null && resources.TryGetValue(Name, out var value))
        {
            return value;
        }

        return Fallback ?? $"[missing token: {Name}]";
    }
}
