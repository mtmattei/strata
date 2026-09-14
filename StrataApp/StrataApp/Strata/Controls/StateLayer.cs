using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace StrataApp.Strata.Controls;

/// <summary>
/// Layer 2. Owns pointer feedback. Knows nothing about shape, fill or content.
/// Remove it from a template and hover/press feedback disappears with zero other edits.
/// </summary>
public partial class StateLayer : ContentControl
{
    public static readonly DependencyProperty IsInteractiveProperty =
        DependencyProperty.Register(
            nameof(IsInteractive),
            typeof(bool),
            typeof(StateLayer),
            new PropertyMetadata(true));

    public bool IsInteractive
    {
        get => (bool)GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public StateLayer()
    {
        DefaultStyleKey = typeof(StateLayer);
        IsTabStop = false;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        GoTo("Normal", false);
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        GoTo("PointerOver");
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        GoTo("Normal");
    }

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);
        GoTo("Pressed");
    }

    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);
        GoTo("PointerOver");
    }

    protected override void OnPointerCanceled(PointerRoutedEventArgs e)
    {
        base.OnPointerCanceled(e);
        GoTo("Normal");
    }

    private void GoTo(string state, bool useTransitions = true)
    {
        if (!IsInteractive)
        {
            VisualStateManager.GoToState(this, "Normal", useTransitions);
            return;
        }

        VisualStateManager.GoToState(this, state, useTransitions);
    }
}
