using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace StrataApp.Strata.Controls;

/// <summary>
/// Layer 1. Owns shape, fill and elevation. Knows nothing about interaction or content.
/// </summary>
public partial class Surface : ContentControl
{
    public static readonly DependencyProperty ElevationProperty =
        DependencyProperty.Register(
            nameof(Elevation),
            typeof(int),
            typeof(Surface),
            new PropertyMetadata(0, OnElevationChanged));

    public int Elevation
    {
        get => (int)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public Surface()
    {
        DefaultStyleKey = typeof(Surface);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ApplyElevationState(useTransitions: false);
    }

    private static void OnElevationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((Surface)d).ApplyElevationState(useTransitions: true);

    private void ApplyElevationState(bool useTransitions)
    {
        var level = Elevation switch
        {
            <= 0 => 0,
            1 => 1,
            2 => 2,
            _ => 3
        };

        VisualStateManager.GoToState(this, $"Elevation{level}", useTransitions);
    }
}
