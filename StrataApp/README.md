# STRATA — layered XAML composition, as a running Uno app

A card built as three nested templated controls, each owning exactly one
concern. Pull the stack apart on screen and every layer accounts for itself.

Uno Platform Single Project, `net10.0-desktop`, Uno.Sdk 6.8.0-dev.12, Skia
renderer. No Material, no Toolkit, no MVUX — stock WinUI primitives plus three
custom controls and one markup extension, so the composition story stays legible.

## Run

```powershell
dotnet build .\StrataApp\StrataApp.csproj -f net10.0-desktop
dotnet run --project .\StrataApp\StrataApp.csproj -f net10.0-desktop
```

Debug builds call `MainWindow.UseStudio()`, which wires the DevServer client for
hot reload. If Uno Studio opens Hot Design over the bench on launch, exit it once
from the flame icon in the top-left — Studio remembers that for the next run.
Removing the `UseStudio()` call disables Hot Design *and* hot reload together.

## What each section demonstrates

**01 · The stack.** Three sibling sheets — Surface, StateLayer, content — that
overlap into one card at separation 0 and tip apart into an exploded deck as you
drag: 56° back, 34° round, 34px of gap per layer. Each sheet's transform is
`{x:Bind SheetMatrix(SepSlider.Value, n)}` — a function binding with a literal
argument, re-evaluated per slider tick, zero converters. The elevation slider
routes only into Surface; the StateLayer toggle kills hover/press without
touching anything else. The dashed amber edge and the three sheet tags are bench
annotation — they fade in with separation via `TagFade`, and are not part of the
component.

**02 · The seam.** Two scoped styles both try `Elevation=3`. The one targeting
`Surface` silently loses — Card's template already sets Elevation on that
Surface through a `TemplateBinding`, and templated properties outrank style
setters in dependency-property precedence. The one targeting `Card` wins,
because it lands upstream of the seam. Verified at runtime: the left card holds
elevation 1, the right renders elevation 3.

**03 · Regions.** Three byte-identical `Card` declarations under three
`Grid.Resources` scopes, resolving to elevation 1 / 0 / 3. The swatch corner
radius resolves through the custom `{markup:Token}` extension.

## Elevation on Skia

`ThemeShadow` renders nothing on the Skia desktop head — an Elevation slider
wired to it looks broken, which is why the Surface template does not use it.
Elevation is three coordinated moves instead, all driven by visual-state setters
on named template parts in `Strata/Themes/Layers.xaml`:

| Level | Lift | Tint opacity | Cast height / opacity |
|-------|------|--------------|-----------------------|
| 0     | 0    | 0            | — / 0                 |
| 1     | -1px | 0.030        | 16px / 0.55           |
| 2     | -3px | 0.055        | 26px / 0.70           |
| 3     | -6px | 0.085        | 40px / 0.85           |

The cast is a gradient band anchored under the bottom edge: plain XAML on Skia
has no blur primitive, so the opaque end of the gradient tucks behind the sheet
and the transparent end fades into the canvas. The lift is a `Margin`, not a
`Translation` — `Translation`'s Z component only feeds `ThemeShadow`.

## The tilt, and why it is a matrix

The explode is a 3D move, and Uno implements neither of the APIs that would
express it directly:

- `UIElement.Transform3D` is **not implemented** (Uno0001), and neither
  `CompositeTransform3D` nor `PerspectiveTransform3D` exists in the assembly —
  XAML referencing them fails to compile.
- `PlaneProjection` **is** implemented and gives real perspective, but it
  composes rotations Z-after-X, the opposite of CSS `rotateX() rotateZ()`. That
  yields a squashed rectangle spun in screen space rather than a sheared plane,
  and its rotation signs are inverted relative to CSS. Nesting one projection
  inside another to recover the CSS order **blanks the entire app** on the Skia
  desktop head — no exception, just a white window.

So `MainPage.SheetMatrix` does the 3D itself. A point `(x, y, gap)` on sheet `n`
maps through `Rx(tilt) · Rz(spin)` to

```
X = x·cos s − y·sin s
Y = (x·sin s + y·cos s)·cos t − gap·sin t
```

which is affine in `x` and `y`, so each sheet collapses to one 2×3 matrix handed
to a plain `MatrixTransform`. Sheet depth survives as the `OffsetY` term and as a
slight perspective scale-up for the sheets nearest the viewer. The one thing an
affine matrix cannot carry is the perspective trapezoid — the sheets stay
parallelograms rather than narrowing toward the far edge.

## Known platform gaps

- `TextBlock.CharacterSpacing` is a silent no-op on the Skia text stack, so the
  `180` on `StrataLabel` does nothing here. The setter is left in place because
  it is correct markup and applies on Windows/WinUI. Measured: "COMPOSITION
  STUDY" lays out at 113px with and without it.
- Uno Studio's Hot Design pauses the running app; drive the bench with Studio
  closed.
- `UIElement.Transform3D` is unimplemented and nested `PlaneProjection` blanks
  the window — see the tilt section above.

## Fonts

The full Super Normal look wants Martian Mono (utility) and Newsreader
(display). Drop static-weight TTFs into `Assets/Fonts/` and repoint
`Font.Utility` / `Font.Display` in `Strata/Themes/Tokens.xaml`:

```xml
<FontFamily x:Key="Font.Utility">ms-appx:///Assets/Fonts/MartianMono-Regular.ttf#Martian Mono</FontFamily>
```

Static weights only — variable TTFs render their default instance on the Skia
text stack.

## Files

```
StrataApp/App.xaml / App.xaml.cs          shell + merged dictionaries
StrataApp/MainPage.xaml / .cs             the specimen bench (3 sections)
StrataApp/Strata/Controls/Surface.cs      layer 1 — shape, fill, elevation
StrataApp/Strata/Controls/StateLayer.cs   layer 2 — pointer feedback
StrataApp/Strata/Controls/Card.cs         layer 3 — content slots
StrataApp/Strata/Markup/TokenExtension.cs {markup:Token} — resolves semantic names
StrataApp/Strata/Themes/Tokens.xaml       palette, shape, spacing, text, instrument chrome
StrataApp/Strata/Themes/Layers.xaml       the three layer templates + variants
```
