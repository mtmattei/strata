using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace StrataApp.Strata.Controls;

/// <summary>
/// Layer 3. Composes Surface + StateLayer + content slots.
/// Contains no shape logic and no pointer logic of its own.
/// </summary>
public partial class Card : ContentControl
{
    public static readonly DependencyProperty ElevationProperty =
        DependencyProperty.Register(
            nameof(Elevation),
            typeof(int),
            typeof(Card),
            new PropertyMetadata(1));

    public static readonly DependencyProperty IsInteractiveProperty =
        DependencyProperty.Register(
            nameof(IsInteractive),
            typeof(bool),
            typeof(Card),
            new PropertyMetadata(true));

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(Card),
            new PropertyMetadata(null));

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(Card),
            new PropertyMetadata(null));

    public int Elevation
    {
        get => (int)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public bool IsInteractive
    {
        get => (bool)GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public DataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public Card()
    {
        DefaultStyleKey = typeof(Card);
    }
}
