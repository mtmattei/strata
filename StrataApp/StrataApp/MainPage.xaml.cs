using System;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;

namespace StrataApp;

public sealed partial class MainPage : Page
{
    /// <summary>Tip-back of the whole deck at full separation, in degrees.</summary>
    private const double TiltDegrees = 56d;

    /// <summary>In-plane spin of the deck at full separation, in degrees.</summary>
    private const double SpinDegrees = -34d;

    /// <summary>Gap between neighbouring sheets along the deck's normal, in pixels.</summary>
    private const double SheetGap = 34d;

    /// <summary>Viewer distance for the perspective foreshortening, in pixels.</summary>
    private const double Perspective = 1500d;

    public MainPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// x:Bind function-binding targets. Each re-evaluates when its argument
    /// path (a Slider's Value) changes — no converters, no INPC plumbing.
    /// </summary>
    public int ToLevel(double value)
        => (int)Math.Round(value, MidpointRounding.AwayFromZero);

    /// <summary>Separation as 0..1.</summary>
    private static double T(double separation) => separation / 100d;

    /// <summary>
    /// The explode is a 3D move the platform will not do for us: Uno implements
    /// neither UIElement.Transform3D nor a nestable PlaneProjection, so the deck
    /// is tipped back by RotationX, spun by RotationZ, and each sheet lifted
    /// along the deck's normal here, then handed to a plain MatrixTransform.
    ///
    /// A point (x, y, gap) on sheet n maps through Rx(tilt) · Rz(spin) to
    ///   X = x·cos s − y·sin s
    ///   Y = (x·sin s + y·cos s)·cos t − gap·sin t
    /// which is affine in x and y, so the whole thing collapses into one 2x3
    /// matrix per sheet. The sheet's own depth survives as the OffsetY term and
    /// as a slight scale-up for the sheets nearest the viewer.
    /// </summary>
    public Matrix SheetMatrix(double separation, int layerIndex)
    {
        var t = T(separation);
        var tilt = TiltDegrees * t * Math.PI / 180d;
        var spin = SpinDegrees * t * Math.PI / 180d;
        var gap = SheetGap * layerIndex * t;

        var cosT = Math.Cos(tilt);
        var sinT = Math.Sin(tilt);
        var cosS = Math.Cos(spin);
        var sinS = Math.Sin(spin);

        // the deck shrinks a little as it tips away; each sheet then gains back
        // the perspective scale it earns by sitting closer to the viewer
        var scale = (1d - t * 0.06d) * (Perspective / (Perspective - gap * cosT));

        return new Matrix(
            m11: scale * cosS,
            m12: scale * sinS * cosT,
            m21: scale * -sinS,
            m22: scale * cosS * cosT,
            offsetX: 0d,
            offsetY: -gap * sinT);
    }

    /// <summary>
    /// Sheet tags and the StateLayer outline are bench annotation, not part of
    /// the component: they hold off until the stack is a third of the way open.
    /// </summary>
    public double TagFade(double separation)
        => Math.Clamp(T(separation) * 1.4d - 0.4d, 0d, 1d);

    public string StageHint(double separation)
        => separation switch
        {
            <= 0 => "ASSEMBLED",
            > 85 => "FULLY EXPLODED",
            _ => "SEPARATING"
        };

    public string ElevCaption(double value)
        => ToLevel(value) switch
        {
            0 => "Flat. Surface reports Elevation0 and drops its shadow.",
            1 => "Resting. A short cast under the bottom edge.",
            2 => "Raised. The default working elevation.",
            _ => "Lifted. Only the Surface layer reacted to this slider."
        };
}
