# Strata

An Uno Platform desktop app that shows a card built from three nested templated controls (`Surface`, `StateLayer`, `Card`), each handling one concern. A slider tilts the layers apart into an exploded stack, and two further sections show how style precedence and resource scopes resolve against the layered template.

## Repository layout

| Path | Contents |
| --- | --- |
| `StrataApp/StrataApp/` | Uno single-project app (`net10.0-desktop`, Skia renderer) |
| `StrataApp/StrataApp/MainPage.xaml` | The specimen bench with its three sections |
| `StrataApp/StrataApp/Strata/Controls/` | `Surface`, `StateLayer` and `Card` templated controls |
| `StrataApp/StrataApp/Strata/Markup/` | `{markup:Token}` markup extension |
| `StrataApp/StrataApp/Strata/Themes/` | `Tokens.xaml` and `Layers.xaml` (control templates) |
| `StrataApp/StrataApp.sln` | Solution file |
| `StrataApp-history.bundle` | Git bundle with the history of the former nested `StrataApp` repo |

## Requirements

- .NET 10 SDK
- Uno.Sdk 6.8.0-dev.12 (pinned in `StrataApp/global.json`, restored automatically)

The app uses stock WinUI controls only: no Material, Toolkit or MVUX.

## Run it

From the repo root:

```bash
dotnet run --project StrataApp/StrataApp/StrataApp.csproj -f net10.0-desktop
```

Debug builds call `UseStudio()` for hot reload. If Hot Design opens over the app on launch, exit it once from the flame icon in the top-left.

## More

See [StrataApp/README.md](StrataApp/README.md) for what each section demonstrates, how elevation is drawn on Skia, why the tilt uses a `MatrixTransform`, known platform gaps and font setup.
